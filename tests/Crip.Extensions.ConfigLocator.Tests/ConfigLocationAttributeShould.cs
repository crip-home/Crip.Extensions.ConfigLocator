namespace Crip.Extensions.ConfigLocator.Tests;

public class ConfigLocationAttributeShould
{
    [Fact]
    public void Constructor_ProperlySetsSectionKey()
    {
        const string sectionKey = "section";

        var subject = new ConfigLocationAttribute(sectionKey);

        subject.SectionKey.Should().Be(sectionKey);
    }

    [Fact]
    public void Constructor_ProperlySetsAdditionalTypes()
    {
        const string sectionKey = "section";
        Type[] types = [typeof(ConfigLocationAttributeShould)];

        var subject = new ConfigLocationAttribute(sectionKey, types);

        subject.SectionKey.Should().Be(sectionKey);
        subject.AdditionalTypes.Should().BeEquivalentTo(types);

        types[0] = typeof(ConfigValidateAttributeShould);
        subject.AdditionalTypes.Should().BeEquivalentTo([typeof(ConfigLocationAttributeShould)]);
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

    [Fact]
    public void Constructor_DefensivelyCopiesAdditionalTypes()
    {
        Type[] additionalTypes = [typeof(ConfigLocationAttributeShould)];

        var subject = new ConfigLocationAttribute("section", additionalTypes);

        additionalTypes[0] = typeof(ConfigValidateAttributeShould);

        subject.AdditionalTypes.Should().BeEquivalentTo([typeof(ConfigLocationAttributeShould)]);
    }
}