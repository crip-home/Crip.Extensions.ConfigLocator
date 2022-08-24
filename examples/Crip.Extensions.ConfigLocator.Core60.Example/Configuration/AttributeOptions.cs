namespace Crip.Extensions.ConfigLocator.Core60.Example.Configuration;

[ConfigLocation("FromAttribute")]
public record AttributeOptions
{
    public string Property { get; set; } = "";
}