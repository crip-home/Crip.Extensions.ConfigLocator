using System.Runtime.CompilerServices;
using Crip.Extensions.ConfigLocator.Generics;

namespace Crip.Extensions.ConfigLocator.Tests.Generics;

public class TypeExtensionsShould
{
    [Theory]
    [InlineData(typeof(NonGeneric), "NonGeneric")]
    [InlineData(typeof(Generic<string>), "Generic")]
    [InlineData(typeof(IEnumerable<string>), "IEnumerable")]
    public void SimpleName(Type type, string name)
    {
        type.SimpleName().Should().Be(name);
    }

    [Theory]
    [InlineData(typeof(PublicClass), true)]
    [InlineData(typeof(AnotherPublicClass), true)]
    [InlineData(typeof(AbstractClass), false)]
    [InlineData(typeof(Generic<int>), false)]
    [InlineData(typeof(SomeStruct), false)]
    [InlineData(typeof(CompilerGenerated), false)]
    [InlineData(typeof(SpecialNameClass), false)]
    public void IsNonAbstractClass(Type type, bool isNonAbstract)
    {
        type.IsNonAbstractClass().Should().Be(isNonAbstract);
    }

    [Theory]
    [InlineData(typeof(CompilerGeneratedAttribute), true)]
    [InlineData(typeof(SpecialNameAttribute), false)]
    public void HasAttribute(Type type, bool hasAttribute)
    {
        typeof(CompilerGenerated).HasAttribute(type).Should().Be(hasAttribute);
    }

    [Fact]
    public void MakeGenericInstance_WithInt()
    {
        const string value = "test";
        var result = typeof(Generic<>).MakeGenericInstance(new[] { typeof(int) }, value);
        result
            .Should().BeOfType(typeof(Generic<int>))
            .And.BeEquivalentTo(new Generic<int>(value));
    }

    [Fact]
    public void MakeGenericInstance_ThrowsInvalidOperationOnNonGeneric()
    {
        Action act = () => typeof(PublicClass).MakeGenericInstance(new[] { typeof(int) });
        act.Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage("Crip.Extensions.ConfigLocator.Tests.Generics.TypeExtensionsShould+PublicClass is not a GenericTypeDefinition. " +
                         "MakeGenericType may only be called on a type for which Type.IsGenericTypeDefinition is true.");
    }

    [Fact]
    public void MakeGenericInstance_ThrowsMissingMethodException()
    {
        Action act = () => typeof(Generic<>).MakeGenericInstance(new[] { typeof(int) });
        act.Should()
            .ThrowExactly<MissingMethodException>()
            .WithMessage("Constructor on type 'Crip.Extensions.ConfigLocator.Tests.Generics.TypeExtensionsShould+Generic*");
    }

    internal class Generic<T>
    {
        public Generic(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    internal class NonGeneric
    {
    }

    public class PublicClass
    {
    }

    public class AnotherPublicClass : PublicClass
    {
    }

    abstract class AbstractClass
    {
    }

    struct SomeStruct
    {
    }

    [CompilerGenerated]
    public class CompilerGenerated
    {
    }

    [SpecialName]
    public class SpecialNameClass
    {
    }
}