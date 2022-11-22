using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using Ngestor.Daemon.Ingestion;
using System.Runtime.InteropServices;

namespace Ngestor.Daemon;
class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddStrategies();
                    services.AddSingleton<IFileSystem, FileSystem>();
                    services.AddHostedService<DriveWatcher>();
                    services.AddSingleton<IngestionService>();
                    services.AddSingleton<IEnvironment, Environment>();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Console.WriteLine("Windows platform");
                        WindowsIntializer.SetupWindowsDependencies(services);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Console.WriteLine("MacOS platform");
                        MacOsInitializerExtensions.SetupMacOsDependencies(services);
                    }
                    else
                    {
                        Console.WriteLine("Generic platform");
                        throw new PlatformNotSupportedException("Only Windows and MacOS are supported, sorry! :(");
                    }

                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(c =>
                    {
                        c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                    });

                    // if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    // {
                    //     logging.SetupMacOsLogging();
                    // }
                });

        var host = builder.Build();

        await host.RunAsync();
    }

}