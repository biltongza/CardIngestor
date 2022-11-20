using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
namespace Ngestor.Daemon;
public static class StrategyInitializer
{
    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.AddByConventionAsSingleton<IIngestionStrategy>();
        return services;
    }
}