# ConfigLocator

![NuGet Version](https://img.shields.io/nuget/v/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=nuget)
![license](https://img.shields.io/github/license/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge)

`Crip.Extensions.ConfigLocator` is a lightweight library for ASP.NET Core that automates the discovery and registration of configuration classes into the Dependency Injection (DI) container. 

Tired of manually adding `services.Configure<TOptions>(...)` for every single options class? `ConfigLocator` handles it for you using simple attributes, keeping your `Program.cs` clean and your configuration organized.

## 🚀 Key Features

- **Auto-Discovery**: Automatically scans assemblies for configuration classes.
- **Attribute-Based**: Link classes to configuration sections directly in the class definition.
- **Validation Support**: Built-in support for Data Annotations and custom `IValidateOptions<T>` validators.
- **Generic Attributes**: Clean syntax for custom validators (C# 11+).
- **Multiple Types**: Bind multiple types to the same configuration section effortlessly.
- **Lean & Fast**: Optimized assembly scanning during startup.

---

## 🛠️ Getting Started

### 1. Installation

Install the package via NuGet:

```bash
dotnet add package Crip.Extensions.ConfigLocator
```

### 2. Setup

In your `Program.cs`, register the configuration locator. By default, it scans the calling assembly:

```csharp
using Crip.Extensions.ConfigLocator;

var builder = WebApplication.CreateBuilder(args);

// Register all options from the calling assembly
builder.Services.AddConfigLocator(builder.Configuration);

// Or specify assemblies to scan
builder.Services.AddConfigLocator(builder.Configuration, typeof(MyOptions).Assembly);
```

---

## 📖 Usage

### 1. Decorate your Options class

Use the `[ConfigLocation]` attribute to specify the configuration section key.

```csharp
using Crip.Extensions.ConfigLocator;

[ConfigLocation("ExternalServices:GitHub")]
public class GitHubOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
```

This class will automatically bind to the following in your `appsettings.json`:

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

### 2. Inject and Use

Inject these options anywhere using standard ASP.NET Core interfaces (`IOptions<T>`, `IOptionsSnapshot<T>`, or `IOptionsMonitor<T>`).

```csharp
public class GitHubService(IOptions<GitHubOptions> options)
{
    private readonly GitHubOptions _options = options.Value;

    public void DoSomething() => Console.WriteLine(_options.ApiKey);
}
```

---

## ✅ Validation

The library seamlessly integrates with ASP.NET Core options validation.

### Data Annotation Validation

Add the `[ConfigValidate]` attribute and use standard `System.ComponentModel.DataAnnotations`:

```csharp
[ConfigLocation("MySection")]
[ConfigValidate] // Enables Data Annotation validation
public class MyOptions
{
    [Required, MinLength(5)]
    public string ApiKey { get; set; } = null!;
}
```

### Custom Validators

For complex logic, provide a custom `IValidateOptions<T>` implementation:

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

// C# 11+ generic attribute syntax
[ConfigLocation("MySection")]
[ConfigValidate<MyOptionsValidator>] 
public class MyOptions
{
    public string ApiKey { get; set; } = null!;
}

// For older C# versions, use: [ConfigValidate(typeof(MyOptionsValidator))]
```

---

## 🧩 Advanced Features

### Multiple Types from Same Section

You can bind multiple types to the same configuration section using a single attribute:

```csharp
[ConfigLocation("ServiceSettings", typeof(AdditionalOptions))]
public class MainOptions
{
    // ...
}
```

---

## ⚠️ Limitations

- **Named Options**: Currently not supported (uses `Options.DefaultName`).
- **Visibility**: Scans for **non-abstract** classes. Supports `public`, `internal`, and `nested` classes.
- **Constructors**: Requires a public parameterless constructor for binding (standard ASP.NET Core requirement).

---

## 🔗 Additional Resources

- [Options pattern in ASP.NET Core (Official Docs)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)
