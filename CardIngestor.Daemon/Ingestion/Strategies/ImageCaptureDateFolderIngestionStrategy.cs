using System.IO.Abstractions;
using ExifLibrary;

public class ImageCaptureDateFolderIngestionStrategy : IIngestionStrategy
{
    private readonly IFileSystem FileSystem;

    public ImageCaptureDateFolderIngestionStrategy(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }
    public async Task<IEnumerable<IngestionOperation>> GetIngestionOperations(IEnumerable<IFileInfo> fileList, IDictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        var operationTasks = fileList.Select(async source =>
        {
            var destinationRoot = parameters["Destination"];
            DateTime imageDate = source.CreationTime;
            var folder = FileSystem.Path.Join(destinationRoot, imageDate.ToString("yyyy_MM_dd"));
            var fullDestinationPath = FileSystem.Path.Join(folder, source.Name);
            var operation = new IngestionOperation(source, fullDestinationPath, overwrite: true, deleteAfterIngest: false); // TODO read overwrite/deleteAfterIngest from config
            return (imageDate, operation);
        });

        var operations = await Task.WhenAll(operationTasks);

        return operations.OrderBy(tuple => tuple.imageDate).Select(tuple => tuple.operation);
    }
}