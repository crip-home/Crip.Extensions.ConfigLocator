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
    /// <param name="assemblies">The collection of <see cref="Assembly"/> to search in.</param>
    extension(IEnumerable<Assembly> assemblies)
    {
        /// <summary>
        /// Get all non-abstract class types from provided assemblies.
        /// </summary>
        /// <returns>The collection of all non-abstract class types.</returns>
        public IEnumerable<Type> GetDefinedTypes() =>
            assemblies.SelectMany(assembly => assembly.GetDefinedTypes());

        /// <summary>
        /// Get all <see cref="Type"/> instances with defined <typeparamref name="TAttribute"/> on them.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to search for.</typeparam>
        /// <returns>The collection of all types with <typeparamref name="TAttribute"/> attribute.</returns>
        public IEnumerable<Type> TypesWithAttribute<TAttribute>()
            where TAttribute : Attribute =>
            assemblies.SelectMany(assembly => assembly.TypesWithAttribute(typeof(TAttribute)));

        /// <summary>
        /// Get all <see cref="Type"/> instances with defined <paramref name="attributeType"/> on them.
        /// </summary>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>The collection of all types with <paramref name="attributeType"/> attribute.</returns>
        public IEnumerable<Type> TypesWithAttribute(Type attributeType) =>
            assemblies.SelectMany(assembly => assembly.TypesWithAttribute(attributeType));
    }

    /// <param name="assembly">The <see cref="Assembly"/> to search in.</param>
    extension(Assembly assembly)
    {
        /// <summary>
        /// Get all non-abstract class types from provided assembly.
        /// </summary>
        /// <returns>The collection of all non-abstract class types.</returns>
        public IEnumerable<Type> GetDefinedTypes() =>
            assembly.GetTypes().Where(type => type.IsNonAbstractClass());

        /// <summary>
        /// Get all <see cref="Type"/> instances with defined <typeparamref name="TAttribute"/> on them.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to search for.</typeparam>
        /// <returns>The collection of all types with <typeparamref name="TAttribute"/> attribute.</returns>
        public IEnumerable<Type> TypesWithAttribute<TAttribute>()
            where TAttribute : Attribute =>
            assembly.TypesWithAttribute(typeof(TAttribute));

        /// <summary>
        /// Get all <see cref="Type"/> instances with defined <paramref name="attributeType"/> on them.
        /// </summary>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>The collection of all types with <paramref name="attributeType"/> attribute.</returns>
        public IEnumerable<Type> TypesWithAttribute(Type attributeType) =>
            assembly.GetDefinedTypes()
                .Where(type => type.HasAttribute(attributeType));
    }
}