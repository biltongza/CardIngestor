using System.IO.Abstractions;

public interface IDriveTypeIdentifier
{
    Task<bool> IsRemovableDrive(IDriveInfo driveInfo);
}