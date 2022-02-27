using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

public class IngestionOrchestratorTests
{
    private readonly DriveWatcher driveWatcherService;
    private readonly TestDriveAttachedNotifier testDriveAttachedNotifier;
    private readonly Mock<IDriveTypeIdentifier> mockDriveTypeIdentifier;
    private readonly MockFileSystem mockFileSystem;
    private readonly Mock<IngestionService> mockIngestionService;

    public IngestionOrchestratorTests()
    {
        var logger = new Mock<ILogger<DriveWatcher>>();
        testDriveAttachedNotifier = new TestDriveAttachedNotifier();
        mockDriveTypeIdentifier = new Mock<IDriveTypeIdentifier>();
        mockFileSystem = new MockFileSystem();
        mockIngestionService = new Mock<IngestionService>(
            new Mock<ILogger<IngestionService>>().Object,
            new Mock<IEnvironment>().Object,
            new List<IIngestionStrategy>(),
            new Mock<ICopyProvider>().Object,
            mockFileSystem
        );
        mockIngestionService.Setup(x => x.Ingest(It.IsAny<IDriveInfo>(), It.IsAny<CancellationToken>()));

        driveWatcherService = new DriveWatcher(
            logger.Object,
            testDriveAttachedNotifier,
            mockDriveTypeIdentifier.Object,
            mockFileSystem,
            mockIngestionService.Object
        );
    }

    [MacOsSpecificFact]
    public async Task ShouldIngestDrivesOnStartup_MacOS()
    {
        mockDriveTypeIdentifier.Setup(x => x.IsRemovableDrive(It.IsAny<IDriveInfo>())).Returns(Task.FromResult(true));
        await driveWatcherService.StartAsync(new CancellationToken());
        mockIngestionService.Verify(x => x.Ingest(It.Is<IDriveInfo>(driveInfo => driveInfo.RootDirectory.FullName == "/:"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [MacOsSpecificFact]
    public async Task ShouldIngestOnDriveAttached_MacOS()
    {
        var rootDirectory = "/Volumes/TestDrive";
        await driveWatcherService.StartAsync(new CancellationToken());
        mockFileSystem.AddDirectory(rootDirectory);
        var driveInfo = mockFileSystem.DriveInfo.FromDriveName("/Volumes/TestDrive");
        testDriveAttachedNotifier.AttachDrive(driveInfo);
        mockIngestionService.Verify(x => x.Ingest(It.Is<IDriveInfo>(driveInfo => driveInfo.RootDirectory.FullName == rootDirectory), It.IsAny<CancellationToken>()), Times.Once);
    }

    [WindowsSpecificFact]
    public async Task ShouldIngestDrivesOnStartup_Windows()
    {
        mockDriveTypeIdentifier.Setup(x => x.IsRemovableDrive(It.IsAny<IDriveInfo>())).Returns(Task.FromResult(true));
        await driveWatcherService.StartAsync(new CancellationToken());
        mockIngestionService.Verify(x => x.Ingest(It.Is<IDriveInfo>(driveInfo => driveInfo.RootDirectory.FullName == @"C:\"), It.IsAny<CancellationToken>()), Times.Once);
    }
}