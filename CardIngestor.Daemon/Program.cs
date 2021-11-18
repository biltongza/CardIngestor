// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IDriveTypeIdentifier, MacOsDriveTypeIdentifier>();
                    services.AddSingleton<IDriveAttachedNotifier, MacOsDriveAttachedNotifier>();
                });

var host = builder.Build();
var id = host.Services.GetRequiredService<IDriveTypeIdentifier>();
var drives = DriveInfo.GetDrives();
foreach (var drive in drives)
{
    var isRemovableDrive = await id.IsRemovableDrive(drive);
    Console.WriteLine($"Drive {drive.Name}: {isRemovableDrive}");
}

var notifier = host.Services.GetRequiredService<IDriveAttachedNotifier>();
notifier.DriveAttached += (sender, e) =>
{
    Console.WriteLine($"Got drive attached notification! {e.DriveInfo.Name}");
};

await host.RunAsync();