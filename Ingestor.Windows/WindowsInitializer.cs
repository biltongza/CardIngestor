using Microsoft.Extensions.DependencyInjection;
using Ingestor.Windows;
public static class WindowsIntializer
{
    public static IServiceCollection SetupWindowsDependencies(this IServiceCollection services)
    {
        services.AddSingleton<ICopyProvider, WindowsCopyProvider>();
        services.AddSingleton<IDriveAttachedNotifier, WindowsDriveAttachedNotifier>();
        services.AddSingleton<IDriveTypeIdentifier, WindowsDriveTypeIdentifier>();
        return services;
    }
}