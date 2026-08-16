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
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers a configuration instance which <paramref name="optionTypes"/> will bind against.
        /// </summary>
        /// <param name="section">The configuration being bound.</param>
        /// <param name="optionTypes">The types of options being configured.</param>
        public void Configure(IConfigurationSection section, params Type[] optionTypes)
        {
            foreach (var type in optionTypes)
            {
                services.Configure(section, type);
            }
        }

        /// <summary>
        /// Registers a configuration instance which <paramref name="optionsType"/> will bind against.
        /// </summary>
        /// <param name="section">The configuration being bound.</param>
        /// <param name="optionsType">The type of options being configured.</param>
        public void Configure(IConfigurationSection section, Type optionsType)
        {
            services.Add(section.GenericOptionsChangeToken(optionsType));
            services.Add(section.GenericConfigureOptions(optionsType));
        }

        /// <summary>
        /// Registers data annotation option validation instance for <paramref name="optionsType"/> type.
        /// </summary>
        /// <param name="optionsType">The type of options being configured.</param>
        public void AddDataAnnotationValidateOptions(Type optionsType) =>
            services.Add(optionsType.GenericDataAnnotationValidateOptions());

        /// <summary>
        /// Registers custom option validation instance for <paramref name="optionsType"/> type.
        /// </summary>
        /// <param name="optionsType">The type of options being configured.</param>
        /// <param name="validatorTypes">The list of custom validator types.</param>
        public void AddCustomValidateOptions(Type optionsType, params Type[] validatorTypes)
        {
            foreach (var validatorType in validatorTypes)
            {
                services.Add(GenericValidateOptions(optionsType, validatorType));
            }
        }
    }

    private static ServiceDescriptor GenericOptionsChangeToken(
        this IConfigurationSection section,
        params Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericOptionsChangeTokenType();
        var instance = section.GenericOptionsChangeTokenInstance(typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }

    private static ServiceDescriptor GenericConfigureOptions(
        this IConfigurationSection section,
        params Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericConfigureOptionsType();
        var instance = section.GenericConfigureOptionsInstance(typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }

    private static ServiceDescriptor GenericDataAnnotationValidateOptions(this Type optionsType)
    {
        var typeArguments = new[] { optionsType };
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