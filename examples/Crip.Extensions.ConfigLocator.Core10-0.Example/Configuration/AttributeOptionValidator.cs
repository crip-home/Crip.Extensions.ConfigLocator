using Microsoft.Extensions.Options;

namespace Crip.Extensions.ConfigLocator.Core100.Example.Configuration;

public class AttributeOptionValidator : IValidateOptions<AttributeOptions>
{
    public ValidateOptionsResult Validate(string? name, AttributeOptions options)
    {
        if (options.Foo == "Bar")
            return ValidateOptionsResult.Fail("Value cannot be an 'Bar'");

        return ValidateOptionsResult.Success;
    }
}
