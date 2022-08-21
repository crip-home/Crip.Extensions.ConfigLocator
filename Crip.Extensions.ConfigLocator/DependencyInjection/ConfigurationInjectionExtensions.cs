using System;
using System.Collections.Generic;
using Crip.Extensions.ConfigLocator.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crip.Extensions.ConfigLocator.DependencyInjection;

public static class ConfigurationInjectionExtensions
{
    public static void CreateGenericOptions(
        this IServiceCollection services,
        IConfigurationSection section,
        IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            services.CreateGenericOptions(section, type);
        }
    }

    public static void CreateGenericOptions(
        this IServiceCollection services,
        IConfigurationSection section,
        Type optionsType)
    {
        var typeArguments = new[] { optionsType };
        services.Add(GenericOptionsChangeToken(section, typeArguments));
        services.Add(GenericConfigureOptions(section, typeArguments));
    }

    private static ServiceDescriptor GenericOptionsChangeToken(
        IConfigurationSection section,
        Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericOptionsChangeTokenType();
        var instance = section.GenericOptionsChangeTokenInstance(typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }

    private static ServiceDescriptor GenericConfigureOptions(
        IConfigurationSection section,
        Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericConfigureOptionsType();
        var instance = section.GenericConfigureOptionsInstance(typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }
}