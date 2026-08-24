using Microsoft.Extensions.DependencyInjection;

namespace Crip.Extensions.ConfigLocator.Tests.Helpers;

public static class ServiceDescriptionExtensions
{
    public static bool Is<TService, TInstance>(this ServiceDescriptor descriptor, ServiceLifetime lifetime) =>
        (
            descriptor.ImplementationType == typeof(TInstance) ||
            descriptor.ImplementationInstance?.GetType() == typeof(TInstance)
        ) &&
        descriptor.ServiceType == typeof(TService) &&
        descriptor.Lifetime == lifetime;
}