using System.IO.Abstractions;

public class IngestionService
{
    private readonly ILogger logger;
    public IngestionService(ILogger<IngestionService> logger)
    {
        this.logger = logger;
    }

    public async virtual Task Ingest(IDriveInfo driveInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("Beginning ingestion on drive {drive}", driveInfo.Name);
    }
}