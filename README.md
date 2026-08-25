# ConfigLocator

![NuGet Version](https://img.shields.io/nuget/v/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=nuget)
![license](https://img.shields.io/github/license/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge)

`Crip.Extensions.ConfigLocator` helps ASP.NET Core apps register options classes automatically.

Instead of scattering `AddOptions<T>().Bind(...)` or `services.Configure<TOptions>(...)` across startup, mark your options with an attribute and let the library do the wiring.

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

The package also ships a project skill to `.github/skills/config-locator-code-review`, which gives Copilot migration and review guidance for this library in consuming repositories.

Register it in `Program.cs`:

```csharp
using Crip.Extensions.ConfigLocator;

var builder = WebApplication.CreateBuilder(args);

// Put config locator registration in the composition root, right after
// the builder is created and before services that consume options.
builder.Services.AddConfigLocator(builder.Configuration);
```

If your attributed options live outside the entry assembly, pass those assemblies explicitly:

```csharp
builder.Services.AddConfigLocator(
    builder.Configuration,
    typeof(MyOptions).Assembly,
    typeof(SharedOptionsMarker).Assembly);
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

## Copilot skill

The package includes a repository skill at `.github/skills/config-locator-code-review/SKILL.md`.

Its guidance is focused on:

- where `AddConfigLocator(...)` should be initialized
- how to migrate from `AddOptions<T>().Bind(...)` to `ConfigLocationAttribute`
- how to review changes and steer owned options types away from manual binding

The packaged MSBuild target copies the skill to the consuming solution root when `$(SolutionDir)` is available, falls back to the repository root when a `.git` directory is found, and finally falls back to the project directory.

You can disable automatic installation in a consuming project by setting:

```xml
<PropertyGroup>
  <ConfigLocatorDisableSkillInstall>true</ConfigLocatorDisableSkillInstall>
</PropertyGroup>
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

This keeps validation on the options type instead of registering `IValidateOptions<T>` manually in `Program.cs`.

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

## Named options

You can register the same options class more than once by giving each registration a name:

```csharp
[ConfigLocation("GitHub:Default")]
[ConfigLocation("GitHub:Tenants:Europe", "europe")]
[ConfigLocation("GitHub:Tenants:Us", "us")]
public class GitHubOptions
{
    public string ApiKey { get; set; } = string.Empty;
}
```

Then resolve a specific named instance:

```csharp
public class GitHubClient(IOptionsMonitor<GitHubOptions> options)
{
    private readonly GitHubOptions _europe = options.Get("europe");
    private readonly GitHubOptions _us = options.Get("us");
}
```

---

## Migrating from manual binding

Prefer this migration path for owned options types:

1. Move the section path to `[ConfigLocation("...")]` on the options class.
2. Move validation to `[ConfigValidate]` or `[ConfigValidate<TValidator>]`.
3. Add `builder.Services.AddConfigLocator(builder.Configuration);` in startup.
4. Remove manual `AddOptions<T>()`, `.Bind(...)`, `Configure<T>(...)`, and manual validator registration for that type.

Example migration:

```csharp
// Before
builder.Services.AddOptions<PaymentsOptions>()
    .Bind(builder.Configuration.GetSection("Payments"))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<IValidateOptions<PaymentsOptions>, PaymentsOptionsValidator>();
```

```csharp
// After
[ConfigLocation("Payments")]
[ConfigValidate<PaymentsOptionsValidator>]
public class PaymentsOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

builder.Services.AddConfigLocator(builder.Configuration);
```

For step-by-step migration and review guidance inside Copilot, use the shipped `config-locator-code-review` skill.

---

## Limitations

- Only non-abstract classes are scanned
- Binding requires a public parameterless constructor

---

## More info

- [Options pattern in ASP.NET Core (Official Docs)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)
