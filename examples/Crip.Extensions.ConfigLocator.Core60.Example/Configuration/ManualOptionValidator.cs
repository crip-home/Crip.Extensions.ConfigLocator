using Microsoft.Extensions.Options;

namespace Crip.Extensions.ConfigLocator.Core60.Example.Configuration;

public class ManualOptionValidator : IValidateOptions<ManualOptions>
{
    public ValidateOptionsResult Validate(string? name, ManualOptions options)
    {
        if (options.Foo == "Bar")
        {
            return ValidateOptionsResult.Fail("Value cannot be an 'Bar'");
        }

        return ValidateOptionsResult.Success;
    }
}