using System.Reflection;
using Microsoft.Extensions.Options;

namespace Crip.Extensions.ConfigLocator.Tests;

public class ConfigLocationAttributeShould
{
    [Fact]
    public void Constructor_ProperlySetsSectionKey()
    {
        const string sectionKey = "section";

        var subject = new ConfigLocationAttribute(sectionKey);

        subject.SectionKey.Should().Be(sectionKey);
        subject.Name.Should().Be(Options.DefaultName);
    }

    [Fact]
    public void Constructor_ProperlySetsAdditionalTypes()
    {
        const string sectionKey = "section";
        Type[] types = [typeof(ConfigLocationAttributeShould)];

        var subject = new ConfigLocationAttribute(sectionKey, types);

        subject.SectionKey.Should().Be(sectionKey);
        subject.Name.Should().Be(Options.DefaultName);
        subject.AdditionalTypes.Should().BeEquivalentTo(types);

        types[0] = typeof(ConfigValidateAttributeShould);
        subject.AdditionalTypes.Should().BeEquivalentTo([typeof(ConfigLocationAttributeShould)]);
    }

    [Fact]
    public void Constructor_WithName_ProperlySetsName()
    {
        const string sectionKey = "section";
        const string name = "tenant";

        var subject = new ConfigLocationAttribute(sectionKey, name);

        subject.SectionKey.Should().Be(sectionKey);
        subject.Name.Should().Be(name);
        subject.AdditionalTypes.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithName_AndAdditionalTypes_ProperlySetsValues()
    {
        const string sectionKey = "section";
        const string name = "tenant";
        Type[] types = [typeof(ConfigLocationAttributeShould)];

        var subject = new ConfigLocationAttribute(sectionKey, name, types);

        subject.SectionKey.Should().Be(sectionKey);
        subject.Name.Should().Be(name);
        subject.AdditionalTypes.Should().BeEquivalentTo(types);
    }

    [Fact]
    public void Constructor_WithSingleSectionKey_UsesEmptyAdditionalTypes()
    {
        var subject = new ConfigLocationAttribute("section");

        subject.AdditionalTypes.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ThrowsForInvalidSectionKey(string sectionKey)
    {
        Action act = () => new ConfigLocationAttribute(sectionKey);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithName_ThrowsForInvalidName(string name)
    {
        Action act = () => new ConfigLocationAttribute("section", name);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_DefensivelyCopiesAdditionalTypes()
    {
        Type[] additionalTypes = [typeof(ConfigLocationAttributeShould)];

        var subject = new ConfigLocationAttribute("section", additionalTypes);

        additionalTypes[0] = typeof(ConfigValidateAttributeShould);

        subject.AdditionalTypes.Should().BeEquivalentTo([typeof(ConfigLocationAttributeShould)]);
    }

    [Fact]
    public void AttributeUsage_AllowsMultipleInstances()
    {
        var usage = typeof(ConfigLocationAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        usage.Should().NotBeNull();
        usage!.AllowMultiple.Should().BeTrue();
    }
}