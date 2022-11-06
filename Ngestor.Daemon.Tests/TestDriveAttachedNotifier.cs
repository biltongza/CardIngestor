using System;

public class TestDriveAttachedNotifier : IDriveAttachedNotifier
{
    public event EventHandler<DriveAttachedEventArgs>? DriveAttached;

    public void AttachDrive(IDriveInfo driveInfo)
    {
        if (DriveAttached != null)
        {
            DriveAttached.Invoke(this, new DriveAttachedEventArgs(driveInfo));
        }
    }
}