public class DriveAttachedEventArgs : EventArgs
{
    public DriveInfo DriveInfo { get; init; }
    public DriveAttachedEventArgs(DriveInfo driveInfo)
    {
        DriveInfo = driveInfo;
    }
}

public interface IDriveAttachedNotifier
{
    event EventHandler<DriveAttachedEventArgs> DriveAttached;
}