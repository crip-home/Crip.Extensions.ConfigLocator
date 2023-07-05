using System;

namespace Crip.Extensions.ConfigLocator;

/// <summary>
/// Configuration validation attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ConfigValidateAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigValidateAttribute"/> class.
    /// </summary>
    /// <param name="validators">The list of validator types associated with this options.</param>
    public ConfigValidateAttribute(params Type[] validators)
    {
        Validators = validators;
    }

    /// <summary>
    /// Gets or sets the list of validator types associated with this options.
    /// </summary>
    public Type[] Validators { get; protected set; }
}
