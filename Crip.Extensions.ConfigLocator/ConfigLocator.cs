using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Crip.Extensions.ConfigLocator.DependencyInjection;
using Crip.Extensions.ConfigLocator.Generics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crip.Extensions.ConfigLocator;

/// <summary>
/// Configuration locator service extensions.
/// </summary>
public static class ConfigLocator
{
    /// <summary>
    /// Register all options classes with <see cref="ConfigLocationAttribute"/>
    /// attribute from provided <paramref name="assemblies"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> to read from.</param>
    /// <param name="assemblies">
    /// The collection of <see cref="Assembly"/> where search for classes with
    /// <see cref="ConfigLocationAttribute"/> attribute.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddConfigLocator(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        services.AddOptions();

        foreach (var type in assemblies.TypesWithAttribute<ConfigLocationAttribute>())
        {
            services.AddConfigurationOf(configuration, type);
        }

        return services;
    }

    private static void AddConfigurationOf(
        this IServiceCollection services,
        IConfiguration configuration,
        Type type)
    {
        var attribute = type.GetCustomAttribute<ConfigLocationAttribute>() ?? throw TypeLoadError();
        var section = configuration.GetSection(attribute.SectionKey);
        var types = type.WithAdditionalTypesOf(attribute);

        services.Configure(section, types.ToArray());
    }

    private static IEnumerable<Type> WithAdditionalTypesOf(this Type type, ConfigLocationAttribute attribute)
    {
        yield return type;

        foreach (var additionalType in attribute.AdditionalTypes ?? Type.EmptyTypes)
        {
            yield return additionalType;
        }
    }

    [ExcludeFromCodeCoverage]
    private static ApplicationException TypeLoadError() => new($"Type load error on {nameof(ConfigLocationAttribute)}");
}