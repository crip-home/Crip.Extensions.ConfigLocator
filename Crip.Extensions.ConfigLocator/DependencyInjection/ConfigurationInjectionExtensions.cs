using System;
using Crip.Extensions.ConfigLocator.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crip.Extensions.ConfigLocator.DependencyInjection;

/// <summary>
/// Extensions for configuration dependency injection.
/// </summary>
public static class ConfigurationInjectionExtensions
{
    /// <summary>
    /// Registers a configuration instance which <paramref name="optionTypes"/> will bind against.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="section">The configuration being bound.</param>
    /// <param name="optionTypes">The types of options being configured.</param>
    public static void Configure(
        this IServiceCollection services,
        IConfigurationSection section,
        params Type[] optionTypes)
    {
        foreach (var type in optionTypes)
        {
            services.Configure(section, type);
        }
    }

    /// <summary>
    /// Registers a configuration instance which <paramref name="optionsType"/> will bind against.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="section">The configuration being bound.</param>
    /// <param name="optionsType">The type of options being configured.</param>
    public static void Configure(
        this IServiceCollection services,
        IConfigurationSection section,
        Type optionsType)
    {
        services.Add(GenericOptionsChangeToken(section, optionsType));
        services.Add(GenericConfigureOptions(section, optionsType));
    }

    private static ServiceDescriptor GenericOptionsChangeToken(
        IConfigurationSection section,
        params Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericOptionsChangeTokenType();
        var instance = section.GenericOptionsChangeTokenInstance(typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }

    private static ServiceDescriptor GenericConfigureOptions(
        IConfigurationSection section,
        params Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericConfigureOptionsType();
        var instance = section.GenericConfigureOptionsInstance(typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }
}