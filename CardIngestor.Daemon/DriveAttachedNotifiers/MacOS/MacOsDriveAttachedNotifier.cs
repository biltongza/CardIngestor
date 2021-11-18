using Microsoft.Extensions.Logging;

public class MacOsDriveAttachedNotifier : IDriveAttachedNotifier
{
    public event EventHandler<DriveAttachedEventArgs> DriveAttached;

    public MacOsDriveAttachedNotifier(ILogger<MacOsDriveAttachedNotifier> logger)
    {
        FileSystemWatcher watcher = new FileSystemWatcher("/Volumes/");
        watcher.NotifyFilter = NotifyFilters.DirectoryName;
        watcher.Created += (sender, e) =>
        {
            logger.LogInformation("Got filesystem notification: {name}", e.FullPath);

            if (string.IsNullOrEmpty(e.FullPath))
            {
                return;
            }

            DriveInfo driveInfo;

            try
            {
                driveInfo = new DriveInfo(e.FullPath);
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