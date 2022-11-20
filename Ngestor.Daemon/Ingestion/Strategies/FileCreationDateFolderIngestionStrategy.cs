using System.IO.Abstractions;
namespace Ngestor.Daemon.Ingestion.Strategies;

public class FileCreationDateFolderIngestionStrategy : IIngestionStrategy
{
    private readonly IFileSystem FileSystem;

    public FileCreationDateFolderIngestionStrategy(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }
    public Task<IEnumerable<IngestionOperation>> GetIngestionOperations(IEnumerable<IFileInfo> fileList, IDictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        var operations = fileList.Select(source =>
        {
            var destinationRoot = parameters["Destination"];
            DateTime imageDate = source.CreationTime;
            var folder = FileSystem.Path.Join(destinationRoot, imageDate.ToString("yyyy_MM_dd"));
            var fullDestinationPath = FileSystem.Path.Join(folder, source.Name);
            var operation = new IngestionOperation(source, fullDestinationPath, overwrite: true, deleteAfterIngest: false); // TODO read overwrite/deleteAfterIngest from config
            return (imageDate, operation);
        });

        var orderedOperations = operations.OrderBy(tuple => tuple.imageDate).Select(tuple => tuple.operation);
        return Task.FromResult(orderedOperations);
    }
}