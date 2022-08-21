using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Crip.Extensions.ConfigLocator.Generics;

namespace Crip.Extensions.ConfigLocator.Tests.Generics;

public class AssemblyExtensionsShould
{
    private readonly Assembly _testAssembly = typeof(AssemblyExtensionsShould).Assembly;

    [Fact]
    public void TypesWithAttribute_ReturnsEmptyResultForAssemblyArray()
    {
        var assemblies = new[] { _testAssembly };

        var result = assemblies.TypesWithAttribute<SpecialNameAttribute>();

        result
            .Should().BeNullOrEmpty();
    }

    [Fact]
    public void TypesWithAttribute_GetsTypesForSingleAssembly()
    {
        var result = _testAssembly.TypesWithAttribute<DisplayNameAttribute>().ToList();

        result.Should().NotBeNullOrEmpty().And.HaveCount(1);
        result.First().SimpleName().Should().Be("CompilerGenerated");
        result.First().Should().BeDecoratedWith<DisplayNameAttribute>();
    }

    [Fact]
    public void TypesWithAttribute_ReturnsEmptyResultForSingleAssembly()
    {
        var result = _testAssembly.TypesWithAttribute<SpecialNameAttribute>();

        result
            .Should().BeNullOrEmpty();
    }

    [DisplayName("Compiler Generated")]
    public class CompilerGenerated
    {
    }
}