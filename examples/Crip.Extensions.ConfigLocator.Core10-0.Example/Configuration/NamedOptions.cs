using System.ComponentModel.DataAnnotations;

namespace Crip.Extensions.ConfigLocator.Core100.Example.Configuration;

[ConfigLocation("NamedOptions:Europe", "europe")]
[ConfigLocation("NamedOptions:America", "america")]
[ConfigValidate]
public record NamedOptions
{
    [Required, MinLength(5)]
    public string? Name { get; set; }
}