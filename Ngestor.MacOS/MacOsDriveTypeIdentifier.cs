using System.Diagnostics;
using System.IO.Abstractions;
namespace Ngestor.MacOS;

public class MacOsDriveTypeIdentifier : IDriveTypeIdentifier
{
    private readonly ILogger logger;
    public MacOsDriveTypeIdentifier(ILogger<MacOsDriveTypeIdentifier> logger)
    {
        this.logger = logger;
    }

    public async Task<bool> IsRemovableDrive(IDriveInfo driveInfo)
    {
        var df = new Process();
        df.StartInfo.FileName = "df";
        df.StartInfo.ArgumentList.Add("-H");
        df.StartInfo.RedirectStandardOutput = true;
        var dfStarted = df.Start();
        if (!dfStarted)
        {
            logger.LogWarning("couldn't start df");
            return false;
        }

        var rawDfOutput = await df.StandardOutput.ReadToEndAsync();
        logger.LogTrace("Raw df output for {driveName}\n{rawOutput}", driveInfo.Name, rawDfOutput);

        await df.WaitForExitAsync();

        var matchingDfRecord = rawDfOutput
                                .Split(System.Environment.NewLine)
                                .Select(line => new DfRecord(line))
                                .Where(record => record.IsValid)
                                .FirstOrDefault(record => record.MountedOn!.Equals(driveInfo.Name));

        if (matchingDfRecord == null)
        {
            logger.LogWarning("couldn't find matching df record for {driveName}", driveInfo.Name);
            return false;
        }

        var diskutil = new Process();
        diskutil.StartInfo.FileName = "diskutil";
        diskutil.StartInfo.ArgumentList.Add("info");
        diskutil.StartInfo.ArgumentList.Add(matchingDfRecord.Filesystem!);
        diskutil.StartInfo.RedirectStandardOutput = true;
        var diskutilStarted = diskutil.Start();
        if (!diskutilStarted)
        {
            logger.LogWarning("couldn't start diskutil");
            return false;
        }
        var rawDiskutilOutput = await diskutil.StandardOutput.ReadToEndAsync();
        logger.LogTrace("Raw diskutil output for {driveName}\n{rawOutput}", driveInfo.Name, rawDiskutilOutput);
        await diskutil.WaitForExitAsync();
        var diskUtilOutput = rawDiskutilOutput
                            .Split(System.Environment.NewLine)
                            .Select(line => line.Trim().Split(':'))
                            .Where(x => x.Length == 2)
                            .ToDictionary(x => x[0].Trim(), x => x[1].Trim());

        if (!diskUtilOutput.TryGetValue("Removable Media", out var removableMediaValue))
        {
            logger.LogWarning("didn't find Removable Media key in diskutil output");
            return false;
        }
        return removableMediaValue == "Removable";
    }
}