using Crip.Extensions.ConfigLocator.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Crip.Extensions.ConfigLocator.Tests;

public class ConfigLocatorShould
{
    private static readonly Dictionary<string, string> ConfigurationData = new()
    {
        { "MyOptions:Foo", "Value" },
    };

    private readonly Mock<IServiceCollection> _services = new();
    private readonly IConfiguration _configuration;

    public ConfigLocatorShould()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(ConfigurationData)
            .Build();
    }

    [Fact]
    public void AddConfigLocator_RegistersAllInstancesInDI()
    {
        _services.Object.AddConfigLocator(_configuration, typeof(ConfigLocatorShould).Assembly);
        _services.ContainsSingletonService<IConfigureOptions<MyOptions>, NamedConfigureFromConfigurationOptions<MyOptions>>();
        _services.ContainsSingletonService<IConfigureOptions<MyOtherOptions>, NamedConfigureFromConfigurationOptions<MyOtherOptions>>();
    }

    [ConfigLocation("MyOptions", typeof(MyOtherOptions))]
    public record MyOptions(string Foo = "") : MyOtherOptions(Foo);

    public record MyOtherOptions(string Foo = "");
}