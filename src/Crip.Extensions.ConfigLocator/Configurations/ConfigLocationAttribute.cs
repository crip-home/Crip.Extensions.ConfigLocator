using System;
using System.Linq;

namespace Crip.Extensions.ConfigLocator;

/// <summary>
/// Configuration location attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ConfigLocationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigLocationAttribute"/> class.
    /// </summary>
    /// <param name="sectionKey">Application settings file section key.</param>
    public ConfigLocationAttribute(string sectionKey)
        : this(sectionKey, Array.Empty<Type>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigLocationAttribute"/> class.
    /// </summary>
    /// <param name="sectionKey">Application settings file section key.</param>
    /// <param name="additionalTypes">Additional option types to be registered with value from same section.</param>
    public ConfigLocationAttribute(string sectionKey, params Type[]? additionalTypes)
    {
        SectionKey = ValidateSectionKey(sectionKey);
        AdditionalTypes = ValidateAdditionalTypes(additionalTypes);
    }

    /// <summary>
    /// Gets additional option types to be registered with value from same section.
    /// </summary>
    public Type[] AdditionalTypes { get; }

    /// <summary>
    /// Gets application settings file section key.
    /// </summary>
    public string SectionKey { get; }

    private static string ValidateSectionKey(string sectionKey) =>
        string.IsNullOrWhiteSpace(sectionKey)
            ? throw new ArgumentException("Section key must be provided.", nameof(sectionKey))
            : sectionKey;

    private static Type[] ValidateAdditionalTypes(Type[]? additionalTypes)
    {
        if (additionalTypes is null)
        {
            throw new ArgumentNullException(nameof(additionalTypes));
        }

        if (additionalTypes.Any(type => type is null))
        {
            throw new ArgumentException("Additional types cannot contain null entries.", nameof(additionalTypes));
        }

        return additionalTypes.ToArray();
    }
}
