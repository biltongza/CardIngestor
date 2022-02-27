using System.IO.Abstractions;
namespace Ingestor.Plugin;
public interface IIngestionStrategy
{
    Task<IEnumerable<IngestionOperation>> GetIngestionOperations(IEnumerable<IFileInfo> fileList, IDictionary<string, string> parameters, CancellationToken cancellationToken);
}