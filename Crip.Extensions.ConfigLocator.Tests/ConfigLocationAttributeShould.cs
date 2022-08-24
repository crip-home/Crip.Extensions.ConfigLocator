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
        Type[] types = { typeof(ConfigLocationAttributeShould) };

        var subject = new ConfigLocationAttribute(sectionKey, types);

        subject.SectionKey.Should().Be(sectionKey);
        subject.AdditionalTypes.Should().BeEquivalentTo(types);
    }
}