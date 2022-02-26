using Microsoft.Extensions.DependencyInjection;

public static class WindowsIntializer
{
    public static IServiceCollection SetupWindowsDependencies(this IServiceCollection services)
    {
        services.AddSingleton<ICopyProvider, WindowsCopyProvider>();
        return services;
    }
}