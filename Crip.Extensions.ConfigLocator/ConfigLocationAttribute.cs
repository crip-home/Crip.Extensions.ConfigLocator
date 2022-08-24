using System;

namespace Crip.Extensions.ConfigLocator;

/// <summary>
/// Configuration location attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ConfigLocationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigLocationAttribute"/> class.
    /// </summary>
    /// <param name="sectionKey">Application settings file section key.</param>
    public ConfigLocationAttribute(string sectionKey)
    {
        SectionKey = sectionKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigLocationAttribute"/> class.
    /// </summary>
    /// <param name="sectionKey">Application settings file section key.</param>
    /// <param name="additionalTypes">Additional option types to be registered with value from same section.</param>
    public ConfigLocationAttribute(string sectionKey, params Type[] additionalTypes)
    {
        AdditionalTypes = additionalTypes;
        SectionKey = sectionKey;
    }

    /// <summary>
    /// Gets additional option types to be registered with value from same section.
    /// </summary>
    public Type[]? AdditionalTypes { get; }

    /// <summary>
    /// Gets application settings file section key.
    /// </summary>
    public string SectionKey { get; }
}