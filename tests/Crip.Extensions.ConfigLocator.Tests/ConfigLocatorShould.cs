using System.ComponentModel.DataAnnotations;
using Crip.Extensions.ConfigLocator.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Crip.Extensions.ConfigLocator.Tests;

public class ConfigLocatorShould
{
    private static readonly Dictionary<string, string?> ConfigurationData = new()
    {
        { "MyOptions:Foo", "Value" },
        { "NamedOptions:Default:Foo", "DefaultValue" },
        { "NamedOptions:Europe:Foo", "EuropeValue" },
        { "NamedOptions:Us:Foo", "UsValue" },
        { "ValidatedNamedOptions:Valid:RequiredValue", "Configured" },
    };

    private readonly Mock<IServiceCollection> _services = new();
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(ConfigurationData)
        .Build();

    [Fact]
    public void AddConfigLocator_RegistersAllInstancesInDI()
    {
        _services.Object.AddConfigLocator(_configuration, [typeof(ConfigLocatorShould).Assembly]);
        _services
            .ContainsSingletonService<IConfigureOptions<MyOptions>,
                NamedConfigureFromConfigurationOptions<MyOptions>>();

        _services
            .ContainsSingletonService<IValidateOptions<MyOptions>,
                DataAnnotationValidateOptions<MyOptions>>();

        _services
            .ContainsSingletonService<IConfigureOptions<MyOtherOptions>,
                NamedConfigureFromConfigurationOptions<MyOtherOptions>>();

        _services.ContainsSingletonService<IValidateOptions<FooOptions>, FooOptionValidator>();
        _services.ContainsSingletonService<IValidateOptions<BarOptions>, BarOptionValidator>();
        _services.ContainsSingletonService<IValidateOptions<BarOptions>, Bar2OptionValidator>();
    }

    [Fact]
    public void AddConfigLocator_OverloadRegistersCallingAssembly()
    {
        _services.Object.AddConfigLocator(_configuration);
        _services
            .ContainsSingletonService<IConfigureOptions<MyOptions>,
                NamedConfigureFromConfigurationOptions<MyOptions>>();
    }

    [Fact]
    public void AddConfigLocator_BindsNamedOptionsForSameType()
    {
        var services = new ServiceCollection();
        services.AddConfigLocator(_configuration, [typeof(ConfigLocatorShould).Assembly]);

        using var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<NamedOptions>>();
        var metadata = serviceProvider.GetRequiredService<IOptionsMonitor<NamedOptionsMetadata>>();

        options.CurrentValue.Foo.Should().Be("DefaultValue");
        options.Get("europe").Foo.Should().Be("EuropeValue");
        options.Get("us").Foo.Should().Be("UsValue");
        metadata.Get("us").Foo.Should().Be("UsValue");
    }

    [Fact]
    public void AddConfigLocator_ValidatesNamedOptionsPerRegistration()
    {
        var services = new ServiceCollection();
        services.AddConfigLocator(_configuration, [typeof(ConfigLocatorShould).Assembly]);

        using var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<ValidatedNamedOptions>>();

        options.Get("valid").RequiredValue.Should().Be("Configured");
        FluentActions.Invoking(() => options.Get("invalid"))
            .Should().Throw<OptionsValidationException>();
    }

    [ConfigLocation("MyOptions", typeof(MyOtherOptions))]
    [ConfigValidate]
    public record MyOptions(string Foo = "") : MyOtherOptions(Foo);

    public record MyOtherOptions(string Foo = "");

    [ConfigLocation("NamedOptions:Default")]
    [ConfigLocation("NamedOptions:Europe", "europe")]
    [ConfigLocation("NamedOptions:Us", "us", typeof(NamedOptionsMetadata))]
    public class NamedOptions
    {
        public string Foo { get; set; } = string.Empty;
    }

    public class NamedOptionsMetadata
    {
        public string Foo { get; set; } = string.Empty;
    }

    [ConfigLocation("ValidatedNamedOptions:Valid", "valid")]
    [ConfigLocation("ValidatedNamedOptions:Invalid", "invalid")]
    [ConfigValidate]
    public class ValidatedNamedOptions
    {
        [Required]
        public string RequiredValue { get; init; } = null!;
    }

    [ConfigValidate<FooOptionValidator>]
    public record FooOptions(string Foo = "");

    [ConfigValidate(typeof(BarOptionValidator), typeof(Bar2OptionValidator))]
    public record BarOptions(string Foo = "");

    public class FooOptionValidator : IValidateOptions<FooOptions>
    {
        public ValidateOptionsResult Validate(string? name, FooOptions options)
        {
            return ValidateOptionsResult.Success;
        }
    }

    public class BarOptionValidator : IValidateOptions<BarOptions>
    {
        public ValidateOptionsResult Validate(string? name, BarOptions options)
        {
            return ValidateOptionsResult.Success;
        }
    }

    public class Bar2OptionValidator : BarOptionValidator
    {
    }
}