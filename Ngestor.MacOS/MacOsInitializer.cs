using Microsoft.Extensions.DependencyInjection;
using Ngestor.Plugin;
using Ngestor.MacOS;
namespace Ngestor.Daemon;
public static class MacOsInitializerExtensions
{
    public static IServiceCollection SetupMacOsDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IDriveTypeIdentifier, MacOsDriveTypeIdentifier>();
        services.AddSingleton<IDriveAttachedNotifier, MacOsDriveAttachedNotifier>();
        services.AddSingleton<ICopyProvider, MacOsCopyProvider>();
        return services;
    }

    public static ILoggingBuilder SetupMacOsLogging(this ILoggingBuilder builder)
    {
        builder.AddDarwinLogger(configure =>
        {
            configure.Subsystem = "CardNgestor.Daemon";
        });
        return builder;
    }
}