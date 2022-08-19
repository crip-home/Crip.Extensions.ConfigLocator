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

        var configurationTypes = assemblies.TypesWithAttribute<ConfigLocationAttribute>();

        foreach (var type in configurationTypes)
        {
            var types = new List<Type> { type };
            var attribute = type.GetCustomAttribute<ConfigLocationAttribute>() ?? throw TypeLoadError();
            var section = configuration.GetSection(attribute.SectionKey);
            if (attribute.AdditionalTypes is not null) types.AddRange(attribute.AdditionalTypes);
            services.CreateGenericOptions(section, types);
        }

        return services;
    }

    private static ApplicationException TypeLoadError() => new($"Type load error on {nameof(ConfigLocationAttribute)}");
}