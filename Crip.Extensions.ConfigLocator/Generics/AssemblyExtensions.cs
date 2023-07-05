using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Crip.Extensions.ConfigLocator.Generics;

/// <summary>
/// Extensions for assemblies.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Get all <see cref="Type"/> instances with defined <typeparamref name="TAttribute"/> on them.
    /// </summary>
    /// <param name="assemblies">The collection of <see cref="Assembly"/> to search in.</param>
    /// <typeparam name="TAttribute">The type of attribute to search for.</typeparam>
    /// <returns>The collection of all types with <typeparamref name="TAttribute"/> attribute.</returns>
    public static IEnumerable<Type> TypesWithAttribute<TAttribute>(this IEnumerable<Assembly> assemblies)
        where TAttribute : Attribute =>
        assemblies.SelectMany(assembly => assembly.TypesWithAttribute(typeof(TAttribute)));

    /// <summary>
    /// Get all <see cref="Type"/> instances with defined <paramref name="attributeType"/> on them.
    /// </summary>
    /// <param name="assemblies">The collection of <see cref="Assembly"/> to search in.</param>
    /// <param name="attributeType">The type of attribute to search for.</param>
    /// <returns>The collection of all types with <paramref name="attributeType"/> attribute.</returns>
    public static IEnumerable<Type> TypesWithAttribute(this IEnumerable<Assembly> assemblies, Type attributeType) =>
        assemblies.SelectMany(assembly => assembly.TypesWithAttribute(attributeType));

    /// <summary>
    /// Get all <see cref="Type"/> instances with defined <typeparamref name="TAttribute"/> on them.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to search in.</param>
    /// <typeparam name="TAttribute">The type of attribute to search for.</typeparam>
    /// <returns>The collection of all types with <typeparamref name="TAttribute"/> attribute.</returns>
    public static IEnumerable<Type> TypesWithAttribute<TAttribute>(this Assembly assembly)
        where TAttribute : Attribute =>
        assembly.TypesWithAttribute(typeof(TAttribute));

    /// <summary>
    /// Get all <see cref="Type"/> instances with defined <paramref name="attributeType"/> on them.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to search in.</param>
    /// <param name="attributeType">The type of attribute to search for.</param>
    /// <returns>The collection of all types with <paramref name="attributeType"/> attribute.</returns>
    public static IEnumerable<Type> TypesWithAttribute(this Assembly assembly, Type attributeType) =>
        assembly.DefinedTypes
            .Select(info => info.AsType())
            .Where(type =>
                type.IsNonAbstractClass() &&
                type.HasAttribute(attributeType));
}