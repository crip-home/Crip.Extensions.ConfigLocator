using System;
using System.Collections.Generic;
using System.Reflection;
using Crip.Extensions.ConfigLocator.DependencyInjection;
using Crip.Extensions.ConfigLocator.Generics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crip.Extensions.ConfigLocator;

public static class ConfigLocator
{
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

        services.CreateGenericOptions(section, types);
    }

    private static IEnumerable<Type> WithAdditionalTypesOf(this Type type, ConfigLocationAttribute attribute)
    {
        yield return type;

        foreach (var additionalType in attribute.AdditionalTypes ?? Type.EmptyTypes)
        {
            yield return additionalType;
        }
    }

    private static ApplicationException TypeLoadError() => new($"Type load error on {nameof(ConfigLocationAttribute)}");
}