using System.ComponentModel;
using System.IO.Abstractions;
using System.Runtime.InteropServices;

public class WindowsCopyProvider : ICopyProvider
{
    public bool SupportsProgressNotification => true;

    public event EventHandler<CopyProgressEventArgs>? CopyProgress;

    public async Task Copy(IngestionOperation operation, CancellationToken cancellationToken)
    {
        await CopyAsync(operation.Source.FullName, operation.Destination, operation.Overwrite, cancellationToken);
    }

    // adapted from https://stackoverflow.com/a/27179497/1492861

    private Task CopyAsync(string sourceFileName, string destFileName, bool overwrite, CancellationToken token)
    {
        int pbCancel = 0;
        CopyProgressRoutine copyProgressHandler = (total, transferred, streamSize, streamByteTrans, dwStreamNumber, reason, hSourceFile, hDestinationFile, lpData) =>
        {
            if (this.CopyProgress != null)
            {
                this.CopyProgress.Invoke(this, new CopyProgressEventArgs(sourceFileName, destFileName, total, transferred));
            }
            return CopyProgressResult.PROGRESS_CONTINUE;
        };


        token.ThrowIfCancellationRequested();
        var ctr = token.Register(() => pbCancel = 1);
        var copyTask = Task.Run(() =>
        {
            try
            {
                CopyFileFlags flags = default;

                if (!overwrite)
                {
                    flags |= CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;
                }

                var copied = CopyFileEx(sourceFileName, destFileName, copyProgressHandler, IntPtr.Zero, ref pbCancel, flags);
                if (!copied)
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error);
                }
                token.ThrowIfCancellationRequested();
            }
            finally
            {
                ctr.Dispose();
            }
        }, token);
        return copyTask;
    }

    #region DLL Import

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
       CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel,
       CopyFileFlags dwCopyFlags);

    delegate CopyProgressResult CopyProgressRoutine(
        long totalFileSize,
        long totalBytesTransferred,
        long streamSize,
        long streamBytesTransferred,
        uint dwStreamNumber,
        CopyProgressCallbackReason dwCallbackReason,
        IntPtr hSourceFile,
        IntPtr hDestinationFile,
        IntPtr lpData);

    enum CopyProgressResult : uint
    {
        PROGRESS_CONTINUE = 0,
        PROGRESS_CANCEL = 1,
        PROGRESS_STOP = 2,
        PROGRESS_QUIET = 3
    }

    enum CopyProgressCallbackReason : uint
    {
        CALLBACK_CHUNK_FINISHED = 0x00000000,
        CALLBACK_STREAM_SWITCH = 0x00000001
    }

    [Flags]
    enum CopyFileFlags : uint
    {
        COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
        COPY_FILE_RESTARTABLE = 0x00000002,
        COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
        COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
    }

    #endregion
}