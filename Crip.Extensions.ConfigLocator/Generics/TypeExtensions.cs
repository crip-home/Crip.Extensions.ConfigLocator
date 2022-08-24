using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Crip.Extensions.ConfigLocator.Generics;

/// <summary>
/// Extensions for types.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Get simple name of the type.
    /// </summary>
    /// <param name="type">The type to get name of.</param>
    /// <returns>Simple name of the type (without generic parameters).</returns>
    public static string SimpleName(this Type type)
    {
        var name = type.Name;
        var index = name.IndexOf('`');

        return index == -1 ? name : name.Substring(0, index);
    }

    /// <summary>
    /// Determine is the provided type is not abstract.
    /// </summary>
    /// <param name="type">The type to get value of.</param>
    /// <returns><c>true</c> if type is not abstract, otherwise <c>false</c>.</returns>
    public static bool IsNonAbstractClass(this Type type)
    {
        var typeInfo = type.GetTypeInfo();

        if (typeInfo.IsSpecialName)
        {
            return false;
        }

        if (!typeInfo.IsClass || typeInfo.IsAbstract)
        {
            return false;
        }

        if (typeInfo.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
        {
            return false;
        }

        return typeInfo.IsPublic || typeInfo.IsNestedPublic;
    }

    /// <summary>
    /// Determine whenever the provided type has attribute.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="attribute">The type of attribute to check.</param>
    /// <returns><c>true</c> if type has provided attribute, otherwise <c>false</c>.</returns>
    public static bool HasAttribute(this Type type, Type attribute) =>
        type.GetTypeInfo().IsDefined(attribute, inherit: true);

    /// <summary>
    /// Creates generic instance of the <paramref name="genericInstanceType"/>.
    /// </summary>
    /// <param name="genericInstanceType">The generic type to create.</param>
    /// <param name="typeArguments">The generic type arguments.</param>
    /// <param name="instanceArgs">The arguments provided to create instance.</param>
    /// <returns>Created generic instance.</returns>
    public static object MakeGenericInstance(
        this Type genericInstanceType,
        Type[] typeArguments,
        params object[] instanceArgs)
    {
        var instanceType = genericInstanceType.MakeGenericType(typeArguments);
        return Activator.CreateInstance(instanceType, instanceArgs);
    }
}