using System.IO.Abstractions;

public record IngestionOperation
{
    public IFileInfo Source { get; init; }
    public string Destination { get; init; }
    public bool Overwrite { get; init; }
    public bool DeleteAfterIngest { get; init; }

    public IngestionOperation(IFileInfo source, string destination, bool overwrite, bool deleteAfterIngest)
    {
        Source = source;
        Destination = destination;
        Overwrite = overwrite;
        DeleteAfterIngest = deleteAfterIngest;
    }
}