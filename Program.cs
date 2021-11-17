// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IDriveTypeIdentifier, MacOsDriveTypeIdentifier>();
                });

var host = builder.Build();
var id = host.Services.GetRequiredService<IDriveTypeIdentifier>();
var drives = DriveInfo.GetDrives();
foreach(var drive in drives)
{
    var isRemovableDrive = await id.IsRemovableDrive(drive);
    Console.WriteLine($"Drive {drive.Name}: {isRemovableDrive}");
}

