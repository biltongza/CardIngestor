using System.IO.Abstractions;
namespace Ingestor.Plugin;

public interface ICopyProvider
{
    bool SupportsProgressNotification { get; }
    event EventHandler<CopyProgressEventArgs>? CopyProgress;

    Task Copy(IngestionOperation operation, CancellationToken cancellationToken);
}

public class CopyProgressEventArgs : EventArgs
{
    public long Total { get; init; }
    public long Transferred { get; init; }
    public string Source { get; init; }
    public string Destination { get; init; }
    public CopyProgressEventArgs(string source, string destination, long total, long transferred)
    {
        Source = source;
        Destination = destination;
        Total = total;
        Transferred = transferred;
    }
}