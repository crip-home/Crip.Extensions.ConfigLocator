using System;

namespace Crip.Extensions.ConfigLocator.Exceptions;

/// <summary>
/// Attribute load exception.
/// </summary>
/// <typeparam name="T">The type of the attribute.</typeparam>
public class AttributeLoadException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeLoadException{T}"/> class.
    /// </summary>
    /// <param name="sourceType">The type of the class where load was failed.</param>
    public AttributeLoadException(Type sourceType)
        : base($"Failed to load {typeof(T).Name} from {sourceType.FullName}")
    {
    }
}