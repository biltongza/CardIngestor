namespace Ngestor.Windows;
using System.IO.Abstractions;
using System.Management;
using Microsoft.Extensions.Logging;

public class WindowsDriveAttachedNotifier : IDriveAttachedNotifier, IDisposable
{
    private bool disposedValue;
    private readonly ManagementEventWatcher eventWatcher;
    public event EventHandler<DriveAttachedEventArgs>? DriveAttached;

    public WindowsDriveAttachedNotifier(IFileSystem fileSystem, ILogger<WindowsDriveAttachedNotifier> logger)
    {
        EventQuery query = new EventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_LogicalDisk'");

        eventWatcher = new ManagementEventWatcher();
        eventWatcher.Query = query;
        eventWatcher.EventArrived += (object sender, EventArrivedEventArgs e) =>
        {
            logger.LogDebug("Got WMI event!");
            var instance = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if (instance != null)
            {
                var driveLetter = instance["DeviceID"] as string;
                var driveInfo = fileSystem.DriveInfo.FromDriveName(driveLetter);

                DriveAttached?.Invoke(this, new DriveAttachedEventArgs(driveInfo));
            }
        };
        eventWatcher.Start();
        logger.LogTrace("WMI Event watcher started");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                eventWatcher.Stop();
                eventWatcher.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}