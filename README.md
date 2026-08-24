# ConfigLocator

![NuGet Version](https://img.shields.io/nuget/v/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=nuget)
![license](https://img.shields.io/github/license/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge)

`Crip.Extensions.ConfigLocator` helps ASP.NET Core apps register options classes automatically.

Instead of writing `services.Configure<TOptions>(...)` for every class, mark your options with an attribute and let the library do the wiring.

## What it does

- Finds options classes in your assemblies
- Binds them to configuration sections
- Supports validation
- Can bind more than one type from the same section

---

## Quick start

Install the package:

```bash
dotnet add package Crip.Extensions.ConfigLocator
```

Register it in `Program.cs`:

```csharp
using Crip.Extensions.ConfigLocator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfigLocator(builder.Configuration);
```

---

## Define an options class

```csharp
using Crip.Extensions.ConfigLocator;

[ConfigLocation("ExternalServices:GitHub")]
public class GitHubOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
```

This maps to `appsettings.json`:

```json
{
  "ExternalServices": {
    "GitHub": {
      "ApiKey": "your-api-key",
      "TimeoutSeconds": 60
    }
  }
}
```

Use it like any other ASP.NET Core options class:

```csharp
public class GitHubService(IOptions<GitHubOptions> options)
{
    private readonly GitHubOptions _options = options.Value;

    public void DoSomething() => Console.WriteLine(_options.ApiKey);
}
```

---

## Validation

You can use standard data annotations or a custom validator.

### Data annotations

```csharp
[ConfigLocation("MySection")]
[ConfigValidate]
public class MyOptions
{
    [Required, MinLength(5)]
    public string ApiKey { get; set; } = null!;
}
```

### Custom validator

```csharp
public class MyOptionsValidator : IValidateOptions<MyOptions>
{
    public ValidateOptionsResult Validate(string? name, MyOptions options)
    {
        if (options.ApiKey == "default")
            return ValidateOptionsResult.Fail("API Key cannot be 'default'");

        return ValidateOptionsResult.Success;
    }
}

[ConfigLocation("MySection")]
[ConfigValidate<MyOptionsValidator>]
public class MyOptions
{
    public string ApiKey { get; set; } = null!;
}

// For older C# versions, use:
// [ConfigValidate(typeof(MyOptionsValidator))]
```

---

## More than one type

You can bind multiple types to the same section:

```csharp
[ConfigLocation("ServiceSettings", typeof(AdditionalOptions))]
public class MainOptions
{
    // ...
}
```

---

## Limitations

- Named options are not supported
- Only non-abstract classes are scanned
- Binding requires a public parameterless constructor

---

## More info

- [Options pattern in ASP.NET Core (Official Docs)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)
