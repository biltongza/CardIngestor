using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Logging;

public class IngestionOrchestratorTests
{
    private readonly IngestionOrchestrator driveWatcherService;
    private readonly TestDriveAttachedNotifier testDriveAttachedNotifier;
    private readonly Mock<IDriveTypeIdentifier> mockDriveTypeIdentifier;
    private readonly MockFileSystem mockFileSystem;
    private readonly Mock<IngestionService> mockIngestionService;

    public IngestionOrchestratorTests()
    {
        var logger = new Mock<ILogger<IngestionOrchestrator>>();
        testDriveAttachedNotifier = new TestDriveAttachedNotifier();
        mockDriveTypeIdentifier = new Mock<IDriveTypeIdentifier>();
        mockFileSystem = new MockFileSystem();
        mockIngestionService = new Mock<IngestionService>(new Mock<ILogger<IngestionService>>().Object);
        mockIngestionService.Setup(x => x.Ingest(It.IsAny<IDriveInfo>(), It.IsAny<CancellationToken>()));

        driveWatcherService = new IngestionOrchestrator(
            logger.Object,
            testDriveAttachedNotifier,
            mockDriveTypeIdentifier.Object,
            mockFileSystem,
            mockIngestionService.Object
        );
    }

    [Fact]
    [Trait("OS", "MacOS")]
    public async Task ShouldIngestDrivesOnStartup_MacOS()
    {
        mockDriveTypeIdentifier.Setup(x => x.IsRemovableDrive(It.IsAny<IDriveInfo>())).Returns(Task.FromResult(true));
        await driveWatcherService.StartAsync(new CancellationToken());
        mockIngestionService.Verify(x => x.Ingest(It.Is<IDriveInfo>(driveInfo => driveInfo.RootDirectory.FullName == "/:"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("OS", "MacOS")]
    public async Task ShouldIngestOnDriveAttached_MacOS()
    {
        var rootDirectory = "/Volumes/TestDrive";
        await driveWatcherService.StartAsync(new CancellationToken());
        mockFileSystem.AddDirectory(rootDirectory);
        var driveInfo = mockFileSystem.DriveInfo.FromDriveName("/Volumes/TestDrive");
        testDriveAttachedNotifier.AttachDrive(driveInfo);
        mockIngestionService.Verify(x => x.Ingest(It.Is<IDriveInfo>(driveInfo => driveInfo.RootDirectory.FullName == rootDirectory), It.IsAny<CancellationToken>()), Times.Once);
    }
}