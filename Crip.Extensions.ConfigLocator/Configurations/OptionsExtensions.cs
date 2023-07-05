using System;
using System.Diagnostics.CodeAnalysis;
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
        typeof(IOptionsChangeTokenSource<>).MakeGenericType(typeArgument);

    /// <summary>
    /// Create <see cref="ConfigurationChangeTokenSource{T}"/> instance.
    /// </summary>
    /// <param name="section">The configuration section of the settings.</param>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static object GenericOptionsChangeTokenInstance(this IConfigurationSection section, Type[] typeArguments) =>
        typeof(ConfigurationChangeTokenSource<>)
            .MakeGenericInstance(typeArguments, Options.DefaultName, section);

    /// <summary>
    /// Create <see cref="IConfigureOptions{T}"/> type.
    /// </summary>
    /// <param name="typeArgument">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static Type GenericConfigureOptionsType(this Type typeArgument) =>
        typeof(IConfigureOptions<>).MakeGenericType(typeArgument);

    /// <summary>
    /// Create <see cref="IValidateOptions{T}"/> type.
    /// </summary>
    /// <param name="typeArgument">The type of options type arguments.</param>
    /// <returns>Validate options with provided generic parameter.</returns>
    public static Type GenericValidateOptionsType(this Type typeArgument) =>
        typeof(IValidateOptions<>).MakeGenericType(typeArgument);

    /// <summary>
    /// Create <see cref="NamedConfigureFromConfigurationOptions{T}"/> instance.
    /// </summary>
    /// <param name="section">The configuration section of the settings.</param>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Options with provided generic parameter.</returns>
    public static object GenericConfigureOptionsInstance(this IConfigurationSection section, Type[] typeArguments) =>
        typeof(NamedConfigureFromConfigurationOptions<>)
            .MakeGenericInstance(typeArguments, Options.DefaultName, section, ConfigureBinder);

    /// <summary>
    /// Create <see cref="DataAnnotationValidateOptions{T}"/> instance.
    /// </summary>
    /// <param name="typeArguments">The type of options type arguments.</param>
    /// <returns>Validate options with provided generic parameter.</returns>
    public static object GenericDataAnnotationValidateOptionsInstance(Type[] typeArguments) =>
        typeof(DataAnnotationValidateOptions<>)
            .MakeGenericInstance(typeArguments, Options.DefaultName);
}