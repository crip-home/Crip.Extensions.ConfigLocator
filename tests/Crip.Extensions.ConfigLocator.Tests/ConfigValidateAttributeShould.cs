namespace Crip.Extensions.ConfigLocator.Tests;

public class ConfigValidateAttributeShould
{
    [Fact]
    public void Constructor_ProperlySetsValidators()
    {
        Type[] validators = [typeof(ConfigValidateAttributeShould)];

        var subject = new ConfigValidateAttribute(validators);

        subject.Validators.Should().BeEquivalentTo(validators);

        validators[0] = typeof(ConfigLocationAttributeShould);
        subject.Validators.Should().BeEquivalentTo([typeof(ConfigValidateAttributeShould)]);
    }

    [Fact]
    public void GenericConstructor_ProperlySetsValidators()
    {
        var subject = new ConfigValidateAttribute<ConfigValidateAttributeShould>();

        subject.Validators.Should().BeEquivalentTo([typeof(ConfigValidateAttributeShould)]);
    }

    [Fact]
    public void Constructor_ThrowsForNullValidators()
    {
        Action act = () => new ConfigValidateAttribute(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_DefensivelyCopiesValidators()
    {
        Type[] validators = [typeof(ConfigValidateAttributeShould)];

        var subject = new ConfigValidateAttribute(validators);

        validators[0] = typeof(ConfigLocationAttributeShould);

        subject.Validators.Should().BeEquivalentTo([typeof(ConfigValidateAttributeShould)]);
    }
}
