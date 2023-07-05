using System.ComponentModel.DataAnnotations;

namespace Crip.Extensions.ConfigLocator.Core60.Example.Configuration;

[ConfigLocation("FromAttribute")]
[ConfigValidate<AttributeOptionValidator>]
public record AttributeOptions
{
    [Required]
    public string Foo { get; set; } = null!;
}