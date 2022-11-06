using System.IO.Abstractions;

namespace Ngestor.Windows;

public class WindowsDriveTypeIdentifier : IDriveTypeIdentifier
{
    public Task<bool> IsRemovableDrive(IDriveInfo driveInfo)
    {
        return Task.FromResult(driveInfo.DriveType == DriveType.Removable);
    }
}