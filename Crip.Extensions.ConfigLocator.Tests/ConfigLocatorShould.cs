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
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(ConfigurationData)
        .Build();

    [Fact]
    public void AddConfigLocator_RegistersAllInstancesInDI()
    {
        _services.Object.AddConfigLocator(_configuration, typeof(ConfigLocatorShould).Assembly);
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

    [ConfigLocation("MyOptions", typeof(MyOtherOptions))]
    [ConfigValidate]
    public record MyOptions(string Foo = "") : MyOtherOptions(Foo);

    public record MyOtherOptions(string Foo = "");

    [ConfigValidate<FooOptionValidator>]
    public record FooOptions(string Foo = "");

    [ConfigValidate(typeof(BarOptionValidator), typeof(Bar2OptionValidator))]
    public record BarOptions(string Foo = "");

    public class FooOptionValidator : IValidateOptions<FooOptions>
    {
        public ValidateOptionsResult Validate(string name, FooOptions options)
        {
            return ValidateOptionsResult.Success;
        }
    }

    public class BarOptionValidator : IValidateOptions<BarOptions>
    {
        public ValidateOptionsResult Validate(string name, BarOptions options)
        {
            return ValidateOptionsResult.Success;
        }
    }

    public class Bar2OptionValidator : BarOptionValidator
    {
    }
}