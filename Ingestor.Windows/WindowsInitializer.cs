using Microsoft.Extensions.DependencyInjection;
using Ingestor.Plugin;
using Ingestor.Windows;
public static class WindowsIntializer
{
    public static IServiceCollection SetupWindowsDependencies(this IServiceCollection services)
    {
        services.AddSingleton<ICopyProvider, WindowsCopyProvider>();
        return services;
    }
}