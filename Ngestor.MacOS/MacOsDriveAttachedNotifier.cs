using System.IO.Abstractions;
namespace Ngestor.MacOS;

public class MacOsDriveAttachedNotifier : IDriveAttachedNotifier
{
    public event EventHandler<DriveAttachedEventArgs>? DriveAttached;

    public MacOsDriveAttachedNotifier(ILogger<MacOsDriveAttachedNotifier> logger, IFileSystem fileSystem)
    {
        var watcher = fileSystem.FileSystemWatcher.CreateNew("/Volumes/");
        watcher.NotifyFilter = NotifyFilters.DirectoryName;
        watcher.Deleted += (sender, e) =>
        {
            logger.LogInformation("Got filesystem delete notification: {name}", e.FullPath);
        };
        watcher.Renamed += (sender, e) =>
        {
            logger.LogInformation("Got filesystem rename notification: {name}", e.FullPath);
        };
        watcher.Changed += async (sender, e) =>
        {
            logger.LogInformation("Got filesystem changed notification: {name}", e.FullPath);
        };
        watcher.Error += (sender, e) =>
        {
            logger.LogError(e.GetException(), "Error from FileSystemWatcher");
        };
        watcher.Created += async (sender, e) =>
        {
            logger.LogInformation("Got filesystem created notification: {name}", e.FullPath);

            if (string.IsNullOrEmpty(e.FullPath))
            {
                return;
            }

            IDriveInfo driveInfo;
            try
            {
                driveInfo = new DriveInfoWrapper(fileSystem, new DriveInfo(e.FullPath));
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Drive '{name}' does not seem to be valid", e.Name);
                return;
            }

            if (DriveAttached != null)
            {
                await Task.Delay(100);
                DriveAttached.Invoke(this, new DriveAttachedEventArgs(driveInfo));
            }
        };

        watcher.EnableRaisingEvents = true;
    }
}