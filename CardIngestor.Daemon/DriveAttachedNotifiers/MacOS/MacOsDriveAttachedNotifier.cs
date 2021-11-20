using System.IO.Abstractions;

public class MacOsDriveAttachedNotifier : IDriveAttachedNotifier
{
    public event EventHandler<DriveAttachedEventArgs>? DriveAttached;

    public MacOsDriveAttachedNotifier(ILogger<MacOsDriveAttachedNotifier> logger, IFileSystem fileSystem)
    {
        var watcher = fileSystem.FileSystemWatcher.CreateNew("/Volumes/");
        watcher.NotifyFilter = NotifyFilters.DirectoryName;
        watcher.Created += (sender, e) =>
        {
            logger.LogInformation("Got filesystem notification: {name}", e.FullPath);

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
                DriveAttached.Invoke(this, new DriveAttachedEventArgs(driveInfo));
            }
        };

        watcher.EnableRaisingEvents = true;
    }
}