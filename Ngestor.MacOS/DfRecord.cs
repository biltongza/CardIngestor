using System.Text.RegularExpressions;
namespace Ngestor.MacOS;

public record DfRecord
{
    private static readonly Regex Splitter = new Regex("\\s+");
    public string? Filesystem { get; init; }
    public string? Size { get; init; }
    public string? Used { get; init; }
    public string? Avail { get; init; }
    public string? Capacity { get; init; }
    public string? iused { get; init; }
    public string? ifree { get; init; }
    public string? MountedOn { get; init; }
    public bool IsValid { get; init; }

    public DfRecord(string line)
    {
        var parts = Splitter.Split(line);
        if (parts.Length != 9)
        {
            IsValid = false;
            return;
        }

        Filesystem = parts[0];
        Size = parts[1];
        Used = parts[2];
        Avail = parts[3];
        Capacity = parts[4];
        iused = parts[5];
        ifree = parts[6];
        iused = parts[7];
        MountedOn = parts[8];
        IsValid = true;
    }
}