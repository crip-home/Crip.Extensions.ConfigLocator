using System;
using System.Linq;
using Crip.Extensions.ConfigLocator.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        public void Configure(IConfigurationSection section, params Type[]? optionTypes)
        {
            services.Configure(section, Options.DefaultName, optionTypes);
        }

        /// <summary>
        /// Registers a named configuration instance which <paramref name="optionTypes"/> will bind against.
        /// </summary>
        /// <param name="section">The configuration being bound.</param>
        /// <param name="name">The options instance name.</param>
        /// <param name="optionTypes">The types of options being configured.</param>
        public void Configure(IConfigurationSection section, string name, params Type[]? optionTypes)
        {
            ValidateSection(section);
            name = ValidateName(name);
            optionTypes = ValidateOptionTypes(optionTypes);

            foreach (var type in optionTypes)
            {
                services.Configure(section, name, type);
            }
        }

        /// <summary>
        /// Registers a configuration instance which <paramref name="optionsType"/> will bind against.
        /// </summary>
        /// <param name="section">The configuration being bound.</param>
        /// <param name="optionsType">The type of options being configured.</param>
        public void Configure(IConfigurationSection section, Type optionsType)
        {
            services.Configure(section, Options.DefaultName, optionsType);
        }

        /// <summary>
        /// Registers a named configuration instance which <paramref name="optionsType"/> will bind against.
        /// </summary>
        /// <param name="section">The configuration being bound.</param>
        /// <param name="name">The options instance name.</param>
        /// <param name="optionsType">The type of options being configured.</param>
        public void Configure(IConfigurationSection section, string name, Type optionsType)
        {
            ValidateSection(section);
            name = ValidateName(name);
            ValidateOptionType(optionsType);

            services.Add(section.GenericOptionsChangeToken(name, optionsType));
            services.Add(section.GenericConfigureOptions(name, optionsType));
        }

        /// <summary>
        /// Registers data annotation option validation instance for <paramref name="optionsType"/> type.
        /// </summary>
        /// <param name="optionsType">The type of options being configured.</param>
        public void AddDataAnnotationValidateOptions(Type optionsType)
        {
            services.AddDataAnnotationValidateOptions(optionsType, Options.DefaultName);
        }

        /// <summary>
        /// Registers named data annotation option validation instance for <paramref name="optionsType"/> type.
        /// </summary>
        /// <param name="optionsType">The type of options being configured.</param>
        /// <param name="name">The options instance name.</param>
        public void AddDataAnnotationValidateOptions(Type optionsType, string name)
        {
            ValidateOptionType(optionsType);
            services.Add(optionsType.GenericDataAnnotationValidateOptions(name));
        }

        /// <summary>
        /// Registers named data annotation option validation instances for <paramref name="optionsType"/> type.
        /// </summary>
        /// <param name="optionsType">The type of options being configured.</param>
        /// <param name="names">The options instance names.</param>
        public void AddDataAnnotationValidateOptions(Type optionsType, params string[]? names)
        {
            ValidateOptionType(optionsType);
            names = ValidateNames(names);

            foreach (var name in names)
            {
                services.AddDataAnnotationValidateOptions(optionsType, name);
            }
        }

        /// <summary>
        /// Registers custom option validation instance for <paramref name="optionsType"/> type.
        /// </summary>
        /// <param name="optionsType">The type of options being configured.</param>
        /// <param name="validatorTypes">The list of custom validator types.</param>
        public void AddCustomValidateOptions(Type optionsType, params Type[]? validatorTypes)
        {
            ValidateOptionType(optionsType);
            validatorTypes = ValidateValidatorTypes(validatorTypes);

            foreach (var validatorType in validatorTypes)
            {
                services.Add(GenericValidateOptions(optionsType, validatorType));
            }
        }
    }

    private static ServiceDescriptor GenericOptionsChangeToken(
        this IConfigurationSection section,
        string name,
        params Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericOptionsChangeTokenType();
        var instance = section.GenericOptionsChangeTokenInstance(name, typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }

    private static ServiceDescriptor GenericConfigureOptions(
        this IConfigurationSection section,
        string name,
        params Type[] typeArguments)
    {
        var serviceType = typeArguments[0].GenericConfigureOptionsType();
        var instance = section.GenericConfigureOptionsInstance(name, typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }

    private static ServiceDescriptor GenericDataAnnotationValidateOptions(this Type optionsType, string name)
    {
        Type[] typeArguments = [optionsType];
        var serviceType = typeArguments[0].GenericValidateOptionsType();
        var instance = OptionsExtensions.GenericDataAnnotationValidateOptionsInstance(name, typeArguments);

        return new ServiceDescriptor(serviceType, instance);
    }

    private static ServiceDescriptor GenericValidateOptions(Type optionsType, Type validatorType)
    {
        ValidateOptionType(optionsType);
        ValidateOptionType(validatorType);

        var serviceType = optionsType.GenericValidateOptionsType();

        return new ServiceDescriptor(serviceType, validatorType, ServiceLifetime.Singleton);
    }

    private static void ValidateSection(IConfigurationSection section)
    {
        if (section is null)
        {
            throw new ArgumentNullException(nameof(section));
        }
    }

    private static void ValidateOptionType(Type optionType)
    {
        if (optionType is null)
        {
            throw new ArgumentNullException(nameof(optionType));
        }
    }

    private static Type[] ValidateOptionTypes(Type[]? optionTypes)
    {
        if (optionTypes is null)
        {
            throw new ArgumentNullException(nameof(optionTypes));
        }

        if (optionTypes.Any(type => type is null))
        {
            throw new ArgumentException("Option types cannot contain null entries.", nameof(optionTypes));
        }

        return optionTypes;
    }

    private static string ValidateName(string name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (name != Options.DefaultName && string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Options name cannot be blank.", nameof(name));
        }

        return name;
    }

    private static string[] ValidateNames(string[]? names)
    {
        if (names is null)
        {
            throw new ArgumentNullException(nameof(names));
        }

        if (names.Any(name => name is null))
        {
            throw new ArgumentException("Options names cannot contain null entries.", nameof(names));
        }

        return names.Select(ValidateName).ToArray();
    }

    private static Type[] ValidateValidatorTypes(Type[]? validatorTypes)
    {
        if (validatorTypes is null)
        {
            throw new ArgumentNullException(nameof(validatorTypes));
        }

        if (validatorTypes.Any(type => type is null))
        {
            throw new ArgumentException("Validator types cannot contain null entries.", nameof(validatorTypes));
        }

        return validatorTypes;
    }
}