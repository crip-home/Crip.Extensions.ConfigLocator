using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Crip.Extensions.ConfigLocator.Generics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Crip.Extensions.ConfigLocator.Configurations;

/// <summary>
/// Extension helper methods for options registration.
/// </summary>
public static class OptionsExtensions
{
    [ExcludeFromCodeCoverage]
    private static Action<BinderOptions> ConfigureBinder => _ => { };

    /// <summary>
    /// Create <see cref="IOptionsChangeTokenSource{T}"/> type.
    /// </summary>
    /// <param name="typeArgument">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static Type GenericOptionsChangeTokenType(this Type typeArgument) =>
        typeof(IOptionsChangeTokenSource<>).MakeGenericType(ValidateTypeArgument(typeArgument));

    /// <summary>
    /// Create <see cref="ConfigurationChangeTokenSource{T}"/> instance.
    /// </summary>
    /// <param name="section">The configuration section of the settings.</param>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static object GenericOptionsChangeTokenInstance(this IConfigurationSection section, Type[]? typeArguments) =>
        section.GenericOptionsChangeTokenInstance(Options.DefaultName, typeArguments);

    /// <summary>
    /// Create <see cref="ConfigurationChangeTokenSource{T}"/> instance.
    /// </summary>
    /// <param name="section">The configuration section of the settings.</param>
    /// <param name="name">The options instance name.</param>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static object GenericOptionsChangeTokenInstance(this IConfigurationSection section, string name, Type[]? typeArguments) =>
        typeof(ConfigurationChangeTokenSource<>)
            .MakeGenericInstance(ValidateTypeArguments(typeArguments), [ValidateName(name), ValidateSection(section)]);

    /// <summary>
    /// Create <see cref="IConfigureOptions{T}"/> type.
    /// </summary>
    /// <param name="typeArgument">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static Type GenericConfigureOptionsType(this Type typeArgument) =>
        typeof(IConfigureOptions<>).MakeGenericType(ValidateTypeArgument(typeArgument));

    /// <summary>
    /// Create <see cref="IValidateOptions{T}"/> type.
    /// </summary>
    /// <param name="typeArgument">The type of options type arguments.</param>
    /// <returns>Validate options with provided generic parameter.</returns>
    public static Type GenericValidateOptionsType(this Type typeArgument) =>
        typeof(IValidateOptions<>).MakeGenericType(ValidateTypeArgument(typeArgument));

    /// <summary>
    /// Create <see cref="NamedConfigureFromConfigurationOptions{T}"/> instance.
    /// </summary>
    /// <param name="section">The configuration section of the settings.</param>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static object GenericConfigureOptionsInstance(this IConfigurationSection section, Type[]? typeArguments) =>
        section.GenericConfigureOptionsInstance(Options.DefaultName, typeArguments);

    /// <summary>
    /// Create <see cref="NamedConfigureFromConfigurationOptions{T}"/> instance.
    /// </summary>
    /// <param name="section">The configuration section of the settings.</param>
    /// <param name="name">The options instance name.</param>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static object GenericConfigureOptionsInstance(this IConfigurationSection section, string name, Type[]? typeArguments) =>
        typeof(NamedConfigureFromConfigurationOptions<>)
            .MakeGenericInstance(ValidateTypeArguments(typeArguments), [ValidateName(name), ValidateSection(section), ConfigureBinder]);

    /// <summary>
    /// Create <see cref="DataAnnotationValidateOptions{T}"/> instance.
    /// </summary>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Validate options with provided generic parameter.</returns>
    public static object GenericDataAnnotationValidateOptionsInstance(Type[]? typeArguments) =>
        GenericDataAnnotationValidateOptionsInstance(Options.DefaultName, typeArguments);

    /// <summary>
    /// Create <see cref="DataAnnotationValidateOptions{T}"/> instance.
    /// </summary>
    /// <param name="name">The options instance name.</param>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Validate options with provided generic parameter.</returns>
    public static object GenericDataAnnotationValidateOptionsInstance(string name, Type[]? typeArguments) =>
        typeof(DataAnnotationValidateOptions<>)
            .MakeGenericInstance(ValidateTypeArguments(typeArguments), [ValidateName(name)]);

    private static Type ValidateTypeArgument(Type typeArgument) =>
        typeArgument ?? throw new ArgumentNullException(nameof(typeArgument));

    private static Type[] ValidateTypeArguments(Type[]? typeArguments) =>
        ValidateTypeArgumentsCore(typeArguments);

    private static IConfigurationSection ValidateSection(IConfigurationSection section) =>
        section ?? throw new ArgumentNullException(nameof(section));

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

    private static Type[] ValidateTypeArgumentsCore(Type[]? typeArguments)
    {
        if (typeArguments is null)
        {
            throw new ArgumentNullException(nameof(typeArguments));
        }

        if (typeArguments.Length == 0)
        {
            throw new ArgumentException("At least one type argument is required.", nameof(typeArguments));
        }

        if (typeArguments.Any(type => type is null))
        {
            throw new ArgumentException("Type arguments cannot contain null entries.", nameof(typeArguments));
        }

        return typeArguments;
    }
}