namespace Ngestor.Daemon.Ingestion;
public class GenericCopyProvider : ICopyProvider
{
    public bool SupportsProgressNotification => false;

#pragma warning disable CS0067
    public event EventHandler<CopyProgressEventArgs>? CopyProgress;
#pragma warning restore CS0067
    public Task Copy(IngestionOperation operation, CancellationToken cancellationToken)
    {
        return Task.Run(() => operation.Source.CopyTo(operation.Destination, operation.Overwrite));
    }
}