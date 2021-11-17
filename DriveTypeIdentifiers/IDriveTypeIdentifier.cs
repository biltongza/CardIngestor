public interface IDriveTypeIdentifier
{
    Task<bool> IsRemovableDrive(DriveInfo driveInfo);
}