using System.IO.Abstractions;

namespace Ingestor.Windows;

public class WindowsDriveTypeIdentifier : IDriveTypeIdentifier
{
    public Task<bool> IsRemovableDrive(IDriveInfo driveInfo)
    {
        return Task.FromResult(false);
    }
}