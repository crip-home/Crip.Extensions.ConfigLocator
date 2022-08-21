using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Crip.Extensions.ConfigLocator.Generics;

public static class TypeExtensions
{
    public static string SimpleName(this Type type)
    {
        var name = type.Name;
        var index = name.IndexOf('`');

        return index == -1 ? name : name.Substring(0, index);
    }
    
    public static bool IsNonAbstractClass(this Type type)
    {
        var typeInfo = type.GetTypeInfo();

        if (typeInfo.IsSpecialName)
        {
            return false;
        }

        if (!typeInfo.IsClass || typeInfo.IsAbstract)
        {
            return false;
        }

        if (typeInfo.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
        {
            return false;
        }

        return typeInfo.IsPublic || typeInfo.IsNestedPublic;
    }

    public static bool HasAttribute(this Type type, Type attribute) =>
        type.GetTypeInfo().IsDefined(attribute, inherit: true);

    public static object MakeGenericInstance(
        this Type genericInstanceType,
        Type[] typeArguments,
        params object[] instanceArgs)
    {
        var instanceType = genericInstanceType.MakeGenericType(typeArguments);
        return Activator.CreateInstance(instanceType, instanceArgs);
    }
}