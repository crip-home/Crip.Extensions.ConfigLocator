# Migration guide

Follow this sequence when converting an existing manual options setup to `Crip.Extensions.ConfigLocator`.

## 1. Find the current manual registration

Look for patterns such as:

```csharp
builder.Services.AddOptions<MyOptions>()
    .Bind(builder.Configuration.GetSection("MySection"))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<IValidateOptions<MyOptions>, MyOptionsValidator>();
```

or:

```csharp
builder.Services.Configure<MyOptions>(
    builder.Configuration.GetSection("MySection"));
```

Capture three things before changing anything:

1. The configuration section path.
2. Whether the options are named or unnamed.
3. Whether validation is data-annotation based, custom-validator based, or both.

## 2. Move section knowledge onto the options type

Annotate the owned options type with `ConfigLocationAttribute`.

```csharp
using Crip.Extensions.ConfigLocator;

[ConfigLocation("MySection")]
public class MyOptions
{
    public string ApiKey { get; set; } = string.Empty;
}
```

For named options, use one attribute per binding:

```csharp
[ConfigLocation("GitHub:Default")]
[ConfigLocation("GitHub:Tenants:Europe", "europe")]
[ConfigLocation("GitHub:Tenants:Us", "us")]
public class GitHubOptions
{
    public string ApiKey { get; set; } = string.Empty;
}
```

## 3. Move validation onto the options type

For data annotations:

```csharp
[ConfigLocation("MySection")]
[ConfigValidate]
public class MyOptions
{
    [Required]
    public string ApiKey { get; set; } = null!;
}
```

For a custom validator:

```csharp
[ConfigLocation("MySection")]
[ConfigValidate<MyOptionsValidator>]
public class MyOptions
{
    public string ApiKey { get; set; } = string.Empty;
}
```

This replaces the need to register `IValidateOptions<T>` manually for owned option types.

## 4. Place config locator initialization in the composition root

### Minimal hosting

Put initialization in `Program.cs` immediately after builder creation:

```csharp
using Crip.Extensions.ConfigLocator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfigLocator(builder.Configuration);
```

If your attributed options live in other assemblies:

```csharp
builder.Services.AddConfigLocator(
    builder.Configuration,
    typeof(MyOptions).Assembly,
    typeof(OtherAssemblyMarker).Assembly);
```

### Startup-based hosting

Put initialization in `ConfigureServices`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddConfigLocator(Configuration, typeof(MyOptions).Assembly);
}
```

The important rule is that the call belongs in the app startup path, not inside feature-specific registration methods that hide configuration wiring.

## 5. Remove the manual binding pipeline

Delete manual registration code once the options type is attributed:

- `AddOptions<T>()`
- `.Bind(...)`
- `Configure<T>(...)`
- manual `IValidateOptions<T>` registration that is now covered by `ConfigValidateAttribute`

After migration, the application should rely on:

```csharp
builder.Services.AddConfigLocator(builder.Configuration);
```

## 6. Keep consumption unchanged

Most consumers do not need to change. Keep using the standard options abstractions:

- `IOptions<T>`
- `IOptionsSnapshot<T>`
- `IOptionsMonitor<T>`

## 7. Convert special cases correctly

### Multiple types from one section

```csharp
[ConfigLocation("ServiceSettings", typeof(ServiceMetadataOptions))]
public class ServiceOptions
{
}
```

### Named options

Use repeated attributes instead of repeated `AddOptions<T>(name).Bind(...)` chains.

### Types outside the entry assembly

Pass the assembly list into `AddConfigLocator(...)`. Do not assume the default overload will discover types from referenced class libraries.

## 8. Review the final result

After migration, the preferred shape is:

1. section path declared on the options class
2. validation declared on the options class
3. one startup call to `AddConfigLocator(...)`
4. no new manual `AddOptions<T>().Bind(...)` setup for owned options types
