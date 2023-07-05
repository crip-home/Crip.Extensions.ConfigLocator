using System;
using System.Runtime.Serialization;

namespace Crip.Extensions.ConfigLocator.Exceptions;

/// <summary>
/// Attribute load exception.
/// </summary>
/// <typeparam name="T">The type of the attribute.</typeparam>
[Serializable]
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeLoadException{T}"/> class with serialized data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
    /// </param>
    protected AttributeLoadException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));

        base.GetObjectData(info, context);
    }
}