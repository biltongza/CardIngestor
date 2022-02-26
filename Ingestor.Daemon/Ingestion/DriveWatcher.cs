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
        logger.LogInformation("Starting drive watcher");
        this.cancellationToken = cancellationToken;
        driveAttachedNotifier.DriveAttached += DriveAttachedEventHandler;
        var drives = fileSystem.DriveInfo.GetDrives();
        foreach (var drive in drives)
        {
            var isRemovableDrive = await this.driveTypeIdentifier.IsRemovableDrive(drive);
            if (isRemovableDrive)
            {
                await ingestionService.Ingest(drive, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping drive watcher");
        driveAttachedNotifier.DriveAttached -= DriveAttachedEventHandler;
        this.cancellationToken = null;
        return Task.CompletedTask;
    }


    private async void DriveAttachedEventHandler(object? sender, DriveAttachedEventArgs args)
    {
        if (args.DriveInfo == null)
        {
            return;
        }

        var isRemovable = await this.driveTypeIdentifier.IsRemovableDrive(args.DriveInfo);
        if (isRemovable && this.cancellationToken.HasValue)
        {
            await ingestionService.Ingest(args.DriveInfo, this.cancellationToken.Value);
        }
    }
}