# Review guide

Use this checklist when reviewing changes in projects that use `Crip.Extensions.ConfigLocator`.

## Prefer this shape

```csharp
[ConfigLocation("Payments:Stripe")]
[ConfigValidate<StripeOptionsValidator>]
public class StripeOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

builder.Services.AddConfigLocator(builder.Configuration);
```

## Flag these patterns

Flag new manual binding for owned options types:

```csharp
builder.Services.AddOptions<StripeOptions>()
    .Bind(builder.Configuration.GetSection("Payments:Stripe"));
```

```csharp
builder.Services.Configure<StripeOptions>(
    builder.Configuration.GetSection("Payments:Stripe"));
```

Also flag manual validator registration when the type can use `ConfigValidateAttribute` instead:

```csharp
builder.Services.AddSingleton<IValidateOptions<StripeOptions>, StripeOptionsValidator>();
```

## Review questions

1. Does the project already use `AddConfigLocator(...)` in startup?
2. Is the options type owned by the application and therefore safe to annotate?
3. Is the section path discoverable directly from `ConfigLocationAttribute`?
4. Should validation move to `[ConfigValidate]` or `[ConfigValidate<TValidator>]`?
5. Are named options better represented by multiple `ConfigLocationAttribute` declarations?

## Acceptable exceptions

Do not force migration when:

- the bound type comes from a third-party package
- the configuration shape is runtime-generated and not represented by a stable options class
- the code intentionally performs one-off binding outside the main DI options model

When allowing an exception, ask for an inline explanation or review note so the deviation is intentional and discoverable.

## Suggested review comment

> This project already uses `Crip.Extensions.ConfigLocator`, so this manual `AddOptions<T>().Bind(...)` registration should move to the options type via `[ConfigLocation("...")]`, with startup registration handled by `AddConfigLocator(builder.Configuration)`. That keeps section mapping and validation colocated with the options class and avoids splitting configuration metadata across `Program.cs`.
