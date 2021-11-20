// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IDriveTypeIdentifier, MacOsDriveTypeIdentifier>();
                    services.AddSingleton<IDriveAttachedNotifier, MacOsDriveAttachedNotifier>();
                    services.AddSingleton<IFileSystem, FileSystem>();
                    services.AddHostedService<IngestionOrchestrator>();
                    services.AddSingleton<IngestionService>();
                });

var host = builder.Build();

await host.RunAsync();