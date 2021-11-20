using System.IO.Abstractions;

public class DriveAttachedEventArgs : EventArgs
{
    public IDriveInfo DriveInfo { get; init; }
    public DriveAttachedEventArgs(IDriveInfo driveInfo)
    {
        DriveInfo = driveInfo;
    }
}

public interface IDriveAttachedNotifier
{
    event EventHandler<DriveAttachedEventArgs>? DriveAttached;
}