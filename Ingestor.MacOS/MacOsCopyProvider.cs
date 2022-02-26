using System.IO.Abstractions;
using static Darwin.CopyFile;
namespace Ingestor.MacOS;

public class MacOsCopyProvider : ICopyProvider
{
    public bool SupportsProgressNotification => false; // false until we can stop the crashes

    public event EventHandler<CopyProgressEventArgs>? CopyProgress;
    private readonly IFileSystem FileSystem;
    public MacOsCopyProvider(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public async Task Copy(IngestionOperation operation, CancellationToken cancellationToken)
    {
        using var state = new State();
        // this currently causes a crash
        // see https://github.com/dotnet/runtime/issues/62454

        // state.SetStatusCallback((Progress what, Stage stage, string source, string dest, State state) =>
        // {
        //     if (cancellationToken.IsCancellationRequested)
        //     {
        //         return NextStep.Quit;
        //     }

        //     if (CopyProgress == null)
        //     {
        //         return NextStep.Continue;
        //     }

        //     if (stage == Stage.Progress)
        //     {
        //         CopyProgress.Invoke(this, new CopyProgressEventArgs(operation.Source.FullName, operation.Destination, operation.Source.Length, state.Copied));
        //     }

        //     return NextStep.Continue;
        // });

        var flags = Flags.Clone;
        if (operation.Overwrite)
        {
            flags |= Flags.Unlink;
        }

        var destinationFileInfo = FileSystem.FileInfo.FromFileName(operation.Destination);
        destinationFileInfo.Directory.Create();

        var status = await Task.Run(() => Darwin.CopyFile.Copy(operation.Source.FullName, operation.Destination, Flags.Clone, state));
        if (status != Status.Ok)
        {
            if (status == Status.EACCESS)
            {
                throw new UnauthorizedAccessException("CopyFile returned EACCESS");
            }

            throw new MacOsCopyException(status);
        }
    }

    public class MacOsCopyException : Exception
    {
        public Status Status { get; init; }
        public MacOsCopyException(Status status) => Status = status;

        public override string ToString() => $"Received status {Status}";
    }


}