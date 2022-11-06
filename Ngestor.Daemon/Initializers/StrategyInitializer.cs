using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public static class StrategyInitializer
{
    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.AddByConventionAsSingleton<IIngestionStrategy>();
        return services;
    }
}