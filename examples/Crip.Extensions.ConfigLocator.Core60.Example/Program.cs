using Crip.Extensions.ConfigLocator;
using Crip.Extensions.ConfigLocator.Core60.Example.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Reads all classes with `ConfigLocation` attribute and register them as `IOptions<T>`
builder.Services.AddConfigLocator(builder.Configuration, typeof(AttributeOptions).Assembly);
builder.Services.Configure<DefaultOptions>(builder.Configuration.GetSection("Default"));

var app = builder.Build();

app.MapGet("/", (IOptionsSnapshot<AttributeOptions> attribute, IOptionsSnapshot<DefaultOptions> configuration) =>
    Results.Json(new { Attribute = attribute.Value, Configuration = configuration.Value }));

app.Run();