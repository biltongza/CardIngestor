namespace Ingestor.Windows;

public class WindowsDriveAttachedNotifier : IDriveAttachedNotifier
{
    public event EventHandler<DriveAttachedEventArgs>? DriveAttached;

}