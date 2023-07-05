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

    /// <summary>
    /// Registers data annotation option validation instance for <paramref name="optionsType"/> type.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="optionsType">The type of options being configured.</param>
    public static void AddDataAnnotationValidateOptions(this IServiceCollection services, Type optionsType) =>
        services.Add(GenericDataAnnotationValidateOptions(optionsType));

    /// <summary>
    /// Registers custom option validation instance for <paramref name="optionsType"/> type.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="optionsType">The type of options being configured.</param>
    /// <param name="validatorTypes">The list of custom validator types.</param>
    public static void AddCustomValidateOptions(
        this IServiceCollection services,
        Type optionsType,
        params Type[] validatorTypes)
    {
        foreach (Type validatorType in validatorTypes)
            services.Add(GenericValidateOptions(optionsType, validatorType));
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

    private static ServiceDescriptor GenericDataAnnotationValidateOptions(params Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericValidateOptionsType();
        var instance = OptionsExtensions.GenericDataAnnotationValidateOptionsInstance(typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }

    private static ServiceDescriptor GenericValidateOptions(Type optionsType, Type validatorType)
    {
        var serviceType = optionsType.GenericValidateOptionsType();

        return new ServiceDescriptor(serviceType, validatorType, ServiceLifetime.Singleton);
    }
}