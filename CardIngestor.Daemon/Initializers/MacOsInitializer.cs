using Microsoft.Extensions.DependencyInjection;

public static class MacOsInitializerExtensions
{
    public static IServiceCollection SetupMacOsDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IDriveTypeIdentifier, MacOsDriveTypeIdentifier>();
        services.AddSingleton<IDriveAttachedNotifier, MacOsDriveAttachedNotifier>();
        services.AddSingleton<ICopyProvider, MacOsCopyProvider>();
        return services;
    }
}