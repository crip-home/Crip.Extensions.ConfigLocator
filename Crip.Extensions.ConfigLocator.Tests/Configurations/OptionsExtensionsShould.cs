using System.Diagnostics.CodeAnalysis;
using Crip.Extensions.ConfigLocator.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Crip.Extensions.ConfigLocator.Tests.Configurations;

public class OptionsExtensionsShould
{
    private const string SectionKey = "MyOptions";

    private static readonly Dictionary<string, string> ConfigurationData = new()
    {
        { $"{SectionKey}:{nameof(PublicClass.SomeOption)}", "SomeOptionValue" },
    };

    private readonly IConfiguration _configuration = GetConfiguration();

    [Fact]
    public void GenericOptionsChangeTokenType_CreatesGenericType()
    {
        typeof(PublicClass).GenericOptionsChangeTokenType()
            .Should().NotBeNull()
            .And.Be<IOptionsChangeTokenSource<PublicClass>>();
    }

    [Fact]
    public void GenericOptionsChangeTokenInstance_CreatesGenericInstance()
    {
        var configurationSection = _configuration.GetSection(SectionKey);
        var instance = configurationSection
            .GenericOptionsChangeTokenInstance(new[] { typeof(PublicClass) });

        instance
            .Should().NotBeNull()
            .And.BeOfType<ConfigurationChangeTokenSource<PublicClass>>();
    }

    [Fact]
    public void GenericConfigureOptionsType_CreatesGenericType()
    {
        typeof(PublicClass).GenericConfigureOptionsType()
            .Should().NotBeNull()
            .And.Be<IConfigureOptions<PublicClass>>();
    }

    [Fact]
    public void GenericConfigureOptionsInstance_CreatesGenericInstance()
    {
        var configurationSection = _configuration.GetSection(SectionKey);
        var instance = configurationSection
            .GenericConfigureOptionsInstance(new[] { typeof(PublicClass) });

        instance.Should().NotBeNull()
            .And.BeOfType<NamedConfigureFromConfigurationOptions<PublicClass>>();
    }

    private static IConfiguration GetConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(ConfigurationData)
            .Build();

    [ExcludeFromCodeCoverage]
    public record PublicClass(string SomeOption = "");
}