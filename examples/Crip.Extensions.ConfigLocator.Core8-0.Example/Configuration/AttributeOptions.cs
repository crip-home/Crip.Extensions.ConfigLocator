using System.ComponentModel.DataAnnotations;

namespace Crip.Extensions.ConfigLocator.Core80.Example.Configuration;

[ConfigLocation("FromAttribute")]
[ConfigValidate<AttributeOptionValidator>]
public record AttributeOptions
{
    [Required]
    public string Foo { get; set; } = null!;
}