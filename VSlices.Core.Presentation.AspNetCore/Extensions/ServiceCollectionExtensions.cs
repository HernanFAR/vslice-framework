﻿using System.Net.NetworkInformation;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Presentation.AspNetCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="ISimpleEndpointDefinition"/> to the service collection.
    /// </summary>
    /// <typeparam name="T">The simple endpoint definition to be added</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">Lifetime of the <see cref="ISimpleEndpointDefinition"/> </param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSimpleEndpointDefinition<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : ISimpleEndpointDefinition
    {
        services.Add(new ServiceDescriptor(typeof(ISimpleEndpointDefinition), typeof(T), lifetime));

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IEndpointDefinition"/> as <see cref="ISimpleEndpointDefinition"/> (as well of specified dependencies) to the service collection.
    /// </summary>
    /// <typeparam name="T">The endpoint definition to be added</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">Lifetime of the <see cref="ISimpleEndpointDefinition"/> </param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddEndpointDefinition<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : IEndpointDefinition
    {
        services.Add(new ServiceDescriptor(typeof(ISimpleEndpointDefinition), typeof(T), lifetime));
        T.DefineDependencies(services);

        return services;
    }

    /// <summary>
    /// Scan a specified assembly for <see cref="IEndpointDefinition"/> implementations and adds as <see cref="ISimpleEndpointDefinition"/> (as well of specified dependencies) to the service collection.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">Lifetime of the <see cref="ISimpleEndpointDefinition"/></param>
    /// <typeparam name="TAnchor">Assembly to Scan</typeparam>
    /// <returns>Service collection</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddEndpointDefinitionsFrom<TAnchor>(
        this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var definerTypes = typeof(TAnchor).Assembly.ExportedTypes
            .Where(e => typeof(IEndpointDefinition).IsAssignableFrom(e))
            .Where(e => e is { IsAbstract: false, IsInterface: false });

        foreach (var definerType in definerTypes)
        {
            var defineDependenciesMethod = definerType.GetMethod(nameof(IUseCaseDependencyDefinition.DefineDependencies));

            if (defineDependenciesMethod is null)
            {
                throw new InvalidOperationException($"{definerType.FullName} does not implement {nameof(IUseCaseDependencyDefinition)}");
            }
            
            services.Add(new ServiceDescriptor(typeof(ISimpleEndpointDefinition), definerType, lifetime)); 
            
            defineDependenciesMethod.Invoke(null, new object?[] { services });
        }

        return services;
    }

    public static IServiceCollection AddHandlersFrom<TAnchor>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var definerTypes = typeof(TAnchor).Assembly.ExportedTypes
            .Where(e => e.GetInterfaces().Where(o => o.IsGenericType).Any(o => o.GetGenericTypeDefinition() == typeof(IHandler<,>)))
            .Where(e => e is { IsAbstract: false, IsInterface: false })
            .Select(e => (e, e.GetInterfaces()
                .Where(o => o.IsGenericType)
                .Single(o => o.GetGenericTypeDefinition() == typeof(IHandler<,>))));

        foreach (var (handlerType, handlerInterface) in definerTypes)
        {
            services.Add(new ServiceDescriptor(handlerInterface, handlerType, lifetime));
        }

        return services;
    }
}
