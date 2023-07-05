# Configuration locator

![issues](https://img.shields.io/github/issues/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=appveyor)
![forks](https://img.shields.io/github/forks/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=appveyor)
![stars](https://img.shields.io/github/stars/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=appveyor)
![license](https://img.shields.io/github/license/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=appveyor)

ASP.NET Core bring cool feature like options pattern but did you feel sometimes that is difficult to trace from which
section in `appsettings.json` file this options come from?

To reduce need register each options class separately and better traceability, this library does that automatically.

---

## Getting started

### Installation

Install [Crip.Extensions.ConfigLocator NuGet package](https://www.nuget.org/packages/Crip.Extensions.ConfigLocator),
or [GitHub package](https://github.com/orgs/crip-home/packages?repo_name=Crip.Extensions.ConfigLocator)

### Prerequisites

Register all options providing assemblies where to search for classes with `ConfigLocation` or `ConfigValidate`
attributes.

```csharp
using Crip.Extensions.ConfigLocator;

builder.Services.AddConfigLocator(builder.Configuration, typeof(Program).Assembly);
```

---

## Usage

Decorate options with `ConfigLocation` attribute:

```csharp
using Crip.Extensions.ConfigLocator;

[ConfigLocation("Path:To:Configuration:Section")]
public class MyOptions
{
    public string Property { get; set; }
}
```

Obtain options within DI container:

```csharp
public class MyController
{
    public MyController(
        IOptions<MyOptions> singleton,
        IOptionsSnapshot<TOptions> snapshot,
        IOptionsMonitor<TOptions> monitor)
    {
        //...
    }
}
```

---

## Validation

Option pattern supports multiple validation options. More about
[Options validation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options#options-validation).

### Data annotation validation

Define options with data annotation validation:

```csharp
using Crip.Extensions.ConfigLocator;
using System.ComponentModel.DataAnnotations;

[ConfigLocation("MySection")]
[ConfigValidate]
public class MyOptions
{
    [Required]
    public string Foo { get; set; } = null!;
}
```

When `MySection:Foo` is `null` in `appsettings.json` file, application will throw exception:

```text
Microsoft.Extensions.Options.OptionsValidationException:
 DataAnnotation validation failed for 'AttributeOptions' members: 'Foo' with the error: 'The Property field is required.'.
```

### Custom option validator

Define custom validator and register configuration with it:

```csharp
public class MyOptionsValidator : IValidateOptions<MyOptions>
{
    public ValidateOptionsResult Validate(string? name, MyOptions options)
    {
        if (options.Foo == "Bar")
            return ValidateOptionsResult.Fail("Options 'Foo' value cannot be an 'Bar'");

        return ValidateOptionsResult.Success;
    }
}

[ConfigLocation("MySection")]
[ConfigValidate<MyOptionsValidator>]
// if you under C# 11 or you like to provide more than one validator:
// [ConfigValidate(typeof(MyOptionsValidator))]
public class MyOptions
{
    public string Foo { get; set; } = null!;
}
```

When `MySection:Foo` is `Bar` in `appsettings.json` file, application will throw exception:

```text
Microsoft.Extensions.Options.OptionsValidationException:
 Options 'Foo' value cannot be an 'Bar'
```

### Options as validatable object

As an addition your options class may implement `IValidatableObject` interface for some custom inline validation.

---

## Additional documentation

- [Options pattern in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)

---

## Limitations

> This package does not support named options.
