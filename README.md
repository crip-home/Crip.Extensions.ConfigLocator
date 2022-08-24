# Configuration locator

![issues](https://img.shields.io/github/issues/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=appveyor)
![forks](https://img.shields.io/github/forks/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=appveyor)
![stars](https://img.shields.io/github/stars/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=appveyor)
![license](https://img.shields.io/github/license/crip-home/Crip.Extensions.ConfigLocator?style=for-the-badge&logo=appveyor)

ASP.NET Core bring cool feature
like [options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.1)
but did you feel sometimes that is difficult to trace from witch section in `appsettings.json` file this options come
from?

To reduce need register each options class separately and better traceability, this library does that automatically.

## Installation

Install [Crip.Extensions.ConfigLocator NuGet package](https://www.nuget.org/packages/Crip.Extensions.ConfigLocator),
or [GitHub package](https://github.com/orgs/crip-home/packages?repo_name=Crip.Extensions.ConfigLocator)

## Decorate

Decorate options with `ConfigLocation` attribute:

```csharp
using Crip.Extensions.ConfigLocator;

[ConfigLocation("Path:To:Configuration:Section")]
public class MyOptions {
    public string Property { get; set; }
}
```

## Register

Register all options just providing assemblies where search for classes with `ConfigLocation` attribute.

```csharp
using Crip.Extensions.ConfigLocator;

builder.Services.AddConfigLocator(builder.Configuration, typeof(Startup).Assembly);
```

## Inject

After this, you will be able to use options pattern and inject settings like `IOptions<MyOptions>` or even `IOptionsSnapshot<MyOptions>`.
