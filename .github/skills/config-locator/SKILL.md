---
name: config-locator
description: Guides migration and code review for Crip.Extensions.ConfigLocator. Use this when adding configuration options, placing AddConfigLocator initialization, migrating from AddOptions<T>().Bind(...), or reviewing code that should prefer ConfigLocationAttribute over manual binding.
license: MIT
---

Use this skill whenever work involves configuration options in a project that references `Crip.Extensions.ConfigLocator`.

## Primary goals

1. Prefer the current library API: `ConfigLocationAttribute`, `ConfigValidateAttribute`, and `AddConfigLocator(...)`.
2. Migrate manual configuration wiring to attribute-based registration whenever the options type is owned by the application.
3. Keep startup initialization in the application composition root so all attributed options are registered before they are consumed.
4. During code review, flag new `AddOptions<T>().Bind(...)` or `Configure<T>(...)` patterns unless there is a concrete reason attributes cannot be used.

## Workflow

1. Read `migration-guide.md` and follow the step-by-step migration path.
2. If the task is a review, apply `review-guide.md` and produce feedback that explains why attribute-based registration is preferred.
3. Keep recommendations aligned to the current public API name: `ConfigLocationAttribute`.

## Library-specific rules

- Put `AddConfigLocator(builder.Configuration)` in `Program.cs` immediately after the application builder is created and before services that consume options are registered.
- If attributed option types live outside the entry assembly, pass the relevant assemblies explicitly to `AddConfigLocator(configuration, typeof(SomeOptions).Assembly, ...)`.
- For validation, prefer `[ConfigValidate]` for data annotations and `[ConfigValidate<TValidator>]` for custom validators instead of manual validator registration.
- For named options, use multiple `ConfigLocationAttribute` declarations with names instead of separate manual `AddOptions<T>(name)` chains.
- For one section that should hydrate multiple owned types, use the `additionalTypes` overload on `ConfigLocationAttribute`.

## When manual binding may still be acceptable

Manual binding is a fallback, not the default. Keep it only when the target type cannot practically be annotated, such as:

- a third-party options type you do not control
- a type whose configuration shape is fully dynamic at runtime
- a one-off binding path that is intentionally not part of the application's normal DI options registration

When one of these exceptions applies, explain it explicitly in the review or migration notes.
