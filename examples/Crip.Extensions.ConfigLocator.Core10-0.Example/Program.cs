using Crip.Extensions.ConfigLocator;
using Crip.Extensions.ConfigLocator.Core100.Example.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Reads all classes with `ConfigLocation` attribute and register them as `IOptions<T>`
// By default, it scans the calling assembly (this one).
builder.Services.AddConfigLocator(builder.Configuration);
builder.Services.AddOptions<ManualOptions>()
    .Bind(builder.Configuration.GetSection("Manual"))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<IValidateOptions<ManualOptions>, ManualOptionValidator>();

var app = builder.Build();

app.MapGet("/", (
    IOptionsSnapshot<AttributeOptions> attribute,
    IOptionsSnapshot<ManualOptions> configuration,
    IOptionsMonitor<NamedOptions> named
) => Results.Json(new Dictionary<string, object>
{
    ["attribute"] = attribute.Value,
    ["configuration"] = configuration.Value,
    ["named-europe"] = named.Get("europe"),
    ["named-america"] = named.Get("america"),
}));

app.Run();