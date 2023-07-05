using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Crip.Extensions.ConfigLocator.DependencyInjection;
using Crip.Extensions.ConfigLocator.Exceptions;
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

        foreach (var type in assemblies.TypesWithAttribute<ConfigValidateAttribute>())
        {
            services.AddDataAnnotationValidateOptions(type);
            services.AddCustomValidateOf(type);
        }

        return services;
    }

    private static void AddConfigurationOf(
        this IServiceCollection services,
        IConfiguration configuration,
        Type type)
    {
        var attribute = GetCustomAttribute<ConfigLocationAttribute>(type);
        var section = configuration.GetSection(attribute.SectionKey);
        var types = type.WithAdditionalTypesOf(attribute);

        services.Configure(section, types.ToArray());
    }

    private static void AddCustomValidateOf(
        this IServiceCollection services,
        Type type)
    {
        var attribute = GetCustomAttribute<ConfigValidateAttribute>(type);

        services.AddCustomValidateOptions(type, attribute.Validators);
    }

    private static T GetCustomAttribute<T>(Type type)
        where T : Attribute =>
        type.GetCustomAttribute<T>() ?? throw new AttributeLoadException<T>(type);

    private static IEnumerable<Type> WithAdditionalTypesOf(this Type type, ConfigLocationAttribute attribute)
    {
        yield return type;

        foreach (var additionalType in attribute.AdditionalTypes ?? Type.EmptyTypes)
        {
            yield return additionalType;
        }
    }
}