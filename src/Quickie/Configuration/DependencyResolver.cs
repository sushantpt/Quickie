using Microsoft.Extensions.DependencyInjection;
using Quickie.DataHandlers.Writeonly.Definition;

namespace Quickie.Configuration;

/// <summary>
/// Dependency resolver for data handlers.
/// </summary>
public static class DependencyResolver
{
    
    /*private static IServiceCollection AddCrudDataHandlers(this IServiceCollection services)
    {
        var dataHandlerType = typeof(ICrudDataHandler<,>);
        var implementations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == dataHandlerType));

        foreach (var implementation in implementations)
        {
            var interfaceType = implementation.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == dataHandlerType);
            services.AddScoped(interfaceType, implementation);
        }

        return services;
    }*/
    
    /// <summary>
    /// A more generic di register for data handlers.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="genericInterfaceType"></param>
    private static void RegisterDataHandlers(IServiceCollection services, Type genericInterfaceType)
    {
        var implementations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType));

        foreach (var implementation in implementations)
        {
            var interfaceType = implementation.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType);
            services.AddScoped(interfaceType, implementation);
        }
    }


    /// <summary>
    /// Add data handlers dependency.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns></returns>
    public static IServiceCollection AddDataHandlersDependency(this IServiceCollection services)
    {
        RegisterDataHandlers(services, typeof(ICrudDataHandler<,>));
        RegisterDataHandlers(services, typeof(ICrudForCollectionDataHandler<,>));
        RegisterDataHandlers(services, typeof(IEditOnlyDataHandler<,>));
        RegisterDataHandlers(services, typeof(IReadOnlyDataHandler<,>));
        RegisterDataHandlers(services, typeof(IReadOnlyCollectionDataHandler<,>));
        RegisterDataHandlers(services, typeof(IWriteOnlyDataHandler<>));
        
        return services;
    }
}