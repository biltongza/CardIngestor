using System.IO.Abstractions;
namespace Ingestor.Plugin;
public interface IDriveTypeIdentifier
{
    Task<bool> IsRemovableDrive(IDriveInfo driveInfo);
}