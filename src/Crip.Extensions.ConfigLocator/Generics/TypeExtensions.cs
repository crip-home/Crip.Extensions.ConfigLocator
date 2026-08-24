using System;
using System.Collections.Generic;
using System.Linq;
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
        public object MakeGenericInstance(Type[]? typeArguments, params object[]? instanceArgs)
        {
            typeArguments = ValidateTypeArguments(typeArguments);
            ValidateInstanceArguments(instanceArgs);
            ValidateGenericType(type, typeArguments);

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

    private static void ValidateGenericType(Type type, Type[] typeArguments)
    {
        if (!type.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException($"Type '{type.FullName}' is not a generic type definition.");
        }

        if (type.GetGenericArguments().Length != typeArguments.Length)
        {
            throw new ArgumentException(
                $"Type '{type.FullName}' expects {type.GetGenericArguments().Length} generic type argument(s).",
                nameof(typeArguments));
        }
    }

    private static Type[] ValidateTypeArguments(Type[]? typeArguments)
    {
        if (typeArguments is null)
        {
            throw new ArgumentNullException(nameof(typeArguments));
        }

        if (typeArguments.Length == 0)
        {
            throw new ArgumentException("At least one generic type argument is required.", nameof(typeArguments));
        }

        if (typeArguments.Any(type => type is null))
        {
            throw new ArgumentException("Generic type arguments cannot contain null entries.", nameof(typeArguments));
        }

        return typeArguments;
    }

    private static void ValidateInstanceArguments(object[]? instanceArgs)
    {
        if (instanceArgs is null)
        {
            throw new ArgumentNullException(nameof(instanceArgs));
        }
    }
}