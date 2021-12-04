using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddByConventionAsSingleton<T>(this IServiceCollection services)
    {
        var classes = Assembly.GetAssembly(typeof(Program))!
          .ExportedTypes
          .Where(type => type.IsClass);

        foreach (var implementation in classes)
        {
            var contract = implementation.GetInterfaces()
              .SingleOrDefault(i => i == typeof(T));

            if (contract != null)
            {
                services.AddSingleton(contract, implementation);
            }
        }

        return services;
    }
}