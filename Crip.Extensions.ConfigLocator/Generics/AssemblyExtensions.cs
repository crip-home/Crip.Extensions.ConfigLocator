using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Crip.Extensions.ConfigLocator.Generics;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> TypesWithAttribute<T>(this IEnumerable<Assembly> assemblies)
        where T : Attribute
    {
        return assemblies.SelectMany(assembly => assembly.TypesWithAttribute<T>());
    }

    public static IEnumerable<Type> TypesWithAttribute<T>(this Assembly assembly)
        where T : Attribute
    {
        return assembly.DefinedTypes
            .Select(info => info.AsType())
            .Where(type =>
                type.IsNonAbstractClass() &&
                type.HasAttribute(typeof(T)));
    }
}