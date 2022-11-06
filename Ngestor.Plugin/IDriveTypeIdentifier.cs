using System.IO.Abstractions;
namespace Ngestor.Plugin;
public interface IDriveTypeIdentifier
{
    Task<bool> IsRemovableDrive(IDriveInfo driveInfo);
}