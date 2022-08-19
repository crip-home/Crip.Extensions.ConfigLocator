using System;

namespace Crip.Extensions.ConfigLocator;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigLocationAttribute : Attribute
{
    public Type[]? AdditionalTypes { get; }
    public readonly string SectionKey;

    public ConfigLocationAttribute(string sectionKey)
    {
        SectionKey = sectionKey;
    }

    public ConfigLocationAttribute(string sectionKey, params Type[] additionalTypes)
    {
        AdditionalTypes = additionalTypes;
        SectionKey = sectionKey;
    }
}