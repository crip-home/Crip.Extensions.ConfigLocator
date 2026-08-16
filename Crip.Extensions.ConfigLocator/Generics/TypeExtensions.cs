using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Crip.Extensions.ConfigLocator.Generics;

/// <summary>
/// Extensions for types.
/// </summary>
public static class TypeExtensions
{
    /// <param name="type">The type to get the name of.</param>
    extension(Type type)
    {
        /// <summary>
        /// Get a simple name of the type.
        /// </summary>
        /// <returns>Simple name of the type (without generic parameters).</returns>
        public string SimpleName()
        {
            var name = type.Name;
            var index = name.IndexOf('`');

            return index == -1 ? name : name.Substring(0, index);
        }

        /// <summary>
        /// Determine is the provided type is not abstract.
        /// </summary>
        /// <returns><c>true</c> if type is not abstract, otherwise <c>false</c>.</returns>
        public bool IsNonAbstractClass() =>
            !type.IsSpecialName &&
            !type.IsGenericTypeDefinition &&
            !type.IsInterface &&
            type.IsClass &&
            !type.IsAbstract &&
            !type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true);

        /// <summary>
        /// Determine whenever the provided type has attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to check.</typeparam>
        /// <returns><c>true</c> if type has provided attribute, otherwise <c>false</c>.</returns>
        public bool HasAttribute<TAttribute>()
            where TAttribute : Attribute =>
            type.HasAttribute(typeof(TAttribute));

        /// <summary>
        /// Determine whenever the provided type has attribute.
        /// </summary>
        /// <param name="attribute">The type of attribute to check.</param>
        /// <returns><c>true</c> if type has provided attribute, otherwise <c>false</c>.</returns>
        public bool HasAttribute(Type attribute) =>
            type.IsDefined(attribute, inherit: true);

        /// <summary>
        /// Creates generic instance of the <paramref name="type"/>.
        /// </summary>
        /// <param name="typeArguments">The generic type arguments.</param>
        /// <param name="instanceArgs">The arguments provided to create an instance.</param>
        /// <returns>Created instance.</returns>
        public object MakeGenericInstance(Type[] typeArguments, params object[] instanceArgs)
        {
            var instanceType = type.MakeGenericType(typeArguments);
            return Activator.CreateInstance(instanceType, instanceArgs) ??
                throw new InvalidOperationException($"Failed to create instance of {instanceType.FullName}");
        }
    }

    /// <param name="types">The collection of types to filter.</param>
    extension(IEnumerable<Type> types)
    {
        /// <summary>
        /// Filter collection of types by attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to check.</typeparam>
        /// <returns>The collection of types with provided attribute.</returns>
        public IEnumerable<Type> WithAttribute<TAttribute>()
            where TAttribute : Attribute =>
            types.Where(type => type.HasAttribute<TAttribute>());
    }
}