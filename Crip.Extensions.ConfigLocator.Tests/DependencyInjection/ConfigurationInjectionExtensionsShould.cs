using Crip.Extensions.ConfigLocator.DependencyInjection;
using Crip.Extensions.ConfigLocator.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Crip.Extensions.ConfigLocator.Tests.DependencyInjection;

public class ConfigurationInjectionExtensionsShould
{
    private static readonly Dictionary<string, string> ConfigurationData = new()
    {
        { "MyOptions:Foo", "Value" },
    };

    private readonly Mock<IServiceCollection> _services = new();
    private readonly IConfiguration _configuration;

    public ConfigurationInjectionExtensionsShould()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(ConfigurationData)
            .Build();
    }

    [Fact]
    public void Configure_RegistersOptionsChangeTracking()
    {
        _services.Object.Configure(GetSection(), typeof(MyOptions));
        _services.ContainsSingletonService<IOptionsChangeTokenSource<MyOptions>, ConfigurationChangeTokenSource<MyOptions>>();
    }

    [Fact]
    public void Configure_RegistersOptionsInstance()
    {
        _services.Object.Configure(GetSection(), typeof(MyOptions));
        _services.ContainsSingletonService<IConfigureOptions<MyOptions>, NamedConfigureFromConfigurationOptions<MyOptions>>();
    }

    [Fact]
    public void Configure_RegistersMultipleTypes()
    {
        _services.Object.Configure(GetSection(), typeof(MyOptions), typeof(MyOtherOptions));
        _services.ContainsSingletonService<IConfigureOptions<MyOptions>, NamedConfigureFromConfigurationOptions<MyOptions>>();
        _services.ContainsSingletonService<IConfigureOptions<MyOtherOptions>, NamedConfigureFromConfigurationOptions<MyOtherOptions>>();
    }

    private IConfigurationSection GetSection() =>
        _configuration.GetSection("MyOptions");

    public record MyOptions(string Foo = "default");

    public record MyOtherOptions(string Bar = "default");
}