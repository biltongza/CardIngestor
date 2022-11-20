using System.IO.Abstractions;
using System.Text.Json;
using Ngestor.Daemon.Config;

namespace Ngestor.Daemon.Ingestion;
public class IngestionService
{
    private readonly ILogger logger;
    private readonly IEnvironment environment;
    private readonly IEnumerable<IIngestionStrategy> ingestionStrategies;
    private readonly ICopyProvider copyProvider;
    public IngestionService(
        ILogger<IngestionService> logger,
        IEnvironment environment,
        IEnumerable<IIngestionStrategy> ingestionStrategies,
        ICopyProvider copyProvider,
        IFileSystem fileSystem)
    {
        this.logger = logger;
        this.environment = environment;
        this.ingestionStrategies = ingestionStrategies;
        this.copyProvider = copyProvider;
    }

    public async virtual Task Ingest(IDriveInfo driveInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("Beginning ingestion on drive {drive}", driveInfo.Name);
        var configFileInfo = driveInfo.RootDirectory.EnumerateFiles(".ingest.json").FirstOrDefault();
        if (configFileInfo == null)
        {
            logger.LogInformation("Ingestion config file not found on drive {drive}, skipping.", driveInfo.Name);
            return;
        }

        using var configFileStream = configFileInfo.OpenRead();
        IngestionConfig? config;
        try
        {
            config = await JsonSerializer.DeserializeAsync<IngestionConfig>(configFileStream, ConfigJsonContext.Default.IngestionConfig, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Couldn't deserialize config file.");
            return;
        }

        if (config == null)
        {
            logger.LogError("Config is null?");
            return;
        }

        var matchingDestination = config.Destinations.FirstOrDefault(x => x.MachineName == environment.MachineName);
        if (matchingDestination == null)
        {
            logger.LogInformation("Didn't find a matching destination for machine name {machineName}", environment.MachineName);
            return;
        }

        var matchingStrategy = ingestionStrategies.FirstOrDefault(x => x.GetType().FullName == matchingDestination.IngestionStrategy);
        if (matchingStrategy == null)
        {
            logger.LogError("Didn't find a matching ingestion strategy for {strategyName}", matchingDestination.IngestionStrategy);
            return;
        }

        logger.LogInformation("Getting ingestion operations via strategy {strategyName}", matchingDestination.IngestionStrategy);

        var files = driveInfo.RootDirectory.GetFiles(config.SearchPattern, SearchOption.AllDirectories);

        var operations = await matchingStrategy.GetIngestionOperations(files, matchingDestination.StrategyParams, cancellationToken);


        if (copyProvider.SupportsProgressNotification)
        {
            copyProvider.CopyProgress += (object? sender, CopyProgressEventArgs eventArgs) =>
            {
                logger.LogInformation(
                    "src = \"{src}\", dest = \"{dest}\", progress = {copied}/{total} ({percent}%)",
                    eventArgs.Source,
                    eventArgs.Destination,
                    eventArgs.Transferred,
                    eventArgs.Total,
                    eventArgs.Transferred / eventArgs.Total * 100);

            };

            foreach (var operation in operations)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogWarning("Cancellation requested");
                    return;
                }

                using var logScope = logger.BeginScope(operation);

                logger.LogInformation("Beginning copy from {src} to {dest}", operation.Source.FullName, operation.Destination);
                try
                {
                    await copyProvider.Copy(operation, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to copy {source} to {destination}", operation.Source.FullName, operation.Destination);
                }
                if (operation.DeleteAfterIngest)
                {
                    logger.LogInformation("Deleting source");
                    try
                    {
                        operation.Source.Delete();
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to delete source");
                    }
                }
            }

            logger.LogInformation("Ingest complete");
        }
    }
}