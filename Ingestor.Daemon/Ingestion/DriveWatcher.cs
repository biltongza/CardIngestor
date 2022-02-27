using System.IO.Abstractions;
using Microsoft.Extensions.Hosting;

public class DriveWatcher : IHostedService
{
    private readonly ILogger logger;
    private readonly IDriveAttachedNotifier driveAttachedNotifier;
    private readonly IDriveTypeIdentifier driveTypeIdentifier;
    private readonly IFileSystem fileSystem;
    private readonly IngestionService ingestionService;
    private CancellationToken? cancellationToken;

    public DriveWatcher(
        ILogger<DriveWatcher> logger,
        IDriveAttachedNotifier driveAttachedNotifier,
        IDriveTypeIdentifier driveTypeIdentifier,
        IFileSystem fileSystem,
        IngestionService ingestionService)
    {
        this.logger = logger;
        this.driveAttachedNotifier = driveAttachedNotifier;
        this.driveTypeIdentifier = driveTypeIdentifier;
        this.fileSystem = fileSystem;
        this.ingestionService = ingestionService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Starting drive watcher");
        this.cancellationToken = cancellationToken;
        driveAttachedNotifier.DriveAttached += DriveAttachedEventHandler;
        var drives = fileSystem.DriveInfo.GetDrives();
        foreach (var drive in drives)
        {
            logger.LogTrace("Checking drive {drive}", drive.Name);
            var isRemovableDrive = await this.driveTypeIdentifier.IsRemovableDrive(drive);
            if (isRemovableDrive)
            {
                logger.LogTrace("Drive {drive} is removable, starting ingestion", drive.Name);
                await ingestionService.Ingest(drive, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Stopping drive watcher");
        driveAttachedNotifier.DriveAttached -= DriveAttachedEventHandler;
        this.cancellationToken = null;
        return Task.CompletedTask;
    }


    private async void DriveAttachedEventHandler(object? sender, DriveAttachedEventArgs args)
    {
        logger.LogTrace("Received drive attached event");
        if (args.DriveInfo == null)
        {
            return;
        }

        logger.LogTrace("DriveName = {drive}", args.DriveInfo.Name);
        var isRemovable = await this.driveTypeIdentifier.IsRemovableDrive(args.DriveInfo);
        if (isRemovable && this.cancellationToken.HasValue)
        {
            logger.LogTrace("Drive {drive} is removable, starting ingestion", args.DriveInfo.Name);
            await ingestionService.Ingest(args.DriveInfo, this.cancellationToken!.Value);
        }
    }
}