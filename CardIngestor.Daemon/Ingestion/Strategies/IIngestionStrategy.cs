using System.IO.Abstractions;

public interface IIngestionStrategy
{
    Task<IEnumerable<IngestionOperation>> GetIngestionOperations(IEnumerable<IFileInfo> fileList, IDictionary<string, string> parameters, CancellationToken cancellationToken);
}