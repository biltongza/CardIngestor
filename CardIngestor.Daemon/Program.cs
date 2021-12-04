// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using System.Runtime.InteropServices;

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
                        services.SetupWindowsDependencies();
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        services.SetupMacOsDependencies();
                    }
                    else
                    {
                        throw new PlatformNotSupportedException("Only Windows and MacOS are supported, sorry! :(");
                    }
                });

var host = builder.Build();

await host.RunAsync();