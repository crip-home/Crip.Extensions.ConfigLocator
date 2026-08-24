using System;
using System.Linq;

namespace Crip.Extensions.ConfigLocator;

/// <summary>
/// Configuration validation attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ConfigValidateAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigValidateAttribute"/> class.
    /// </summary>
    /// <param name="validators">The list of validator types associated with this options.</param>
    public ConfigValidateAttribute(params Type[]? validators)
    {
        Validators = ValidateValidators(validators);
    }

    /// <summary>
    /// Gets the list of validator types associated with this options.
    /// </summary>
    public Type[] Validators { get; }

    private static Type[] ValidateValidators(Type[]? validators)
    {
        if (validators is null)
        {
            throw new ArgumentNullException(nameof(validators));
        }

        if (validators.Any(type => type is null))
        {
            throw new ArgumentException("Validators cannot contain null entries.", nameof(validators));
        }

        return validators.ToArray();
    }
}

/// <summary>
/// Configuration validation attribute with validator type.
/// </summary>
/// <typeparam name="TValidator">The type of the custom validator.</typeparam>
public sealed class ConfigValidateAttribute<TValidator> : ConfigValidateAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigValidateAttribute{T}"/> class.
    /// </summary>
    public ConfigValidateAttribute()
        : base(typeof(TValidator))
    {
    }
}
