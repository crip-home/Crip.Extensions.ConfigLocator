namespace Crip.Extensions.ConfigLocator;

/// <summary>
/// Configuration validation attribute with validator type.
/// </summary>
/// <typeparam name="TValidator">The type of the custom validator.</typeparam>
public class ConfigValidateAttribute<TValidator> : ConfigValidateAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigValidateAttribute{T}"/> class.
    /// </summary>
    public ConfigValidateAttribute()
    {
        Validators = new[] { typeof(TValidator) };
    }
}