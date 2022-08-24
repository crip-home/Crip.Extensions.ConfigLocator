using System;
using System.Diagnostics.CodeAnalysis;
using Crip.Extensions.ConfigLocator.Generics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Crip.Extensions.ConfigLocator.Configurations;

public static class OptionsExtensions
{
    [ExcludeFromCodeCoverage]
    private static Action<BinderOptions> ConfigureBinder => _ => { };

    public static Type GenericOptionsChangeTokenType(this Type typeArgument) =>
        typeof(IOptionsChangeTokenSource<>).MakeGenericType(typeArgument);

    public static object GenericOptionsChangeTokenInstance(this IConfigurationSection section, Type[] typeArguments) =>
        typeof(ConfigurationChangeTokenSource<>)
            .MakeGenericInstance(typeArguments, Options.DefaultName, section);

    public static Type GenericConfigureOptionsType(this Type typeArgument) =>
        typeof(IConfigureOptions<>).MakeGenericType(typeArgument);

    public static object GenericConfigureOptionsInstance(this IConfigurationSection section, Type[] typeArguments) =>
        typeof(NamedConfigureFromConfigurationOptions<>)
            .MakeGenericInstance(typeArguments, Options.DefaultName, section, ConfigureBinder);
}