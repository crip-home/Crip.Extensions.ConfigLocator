using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Crip.Extensions.ConfigLocator.Tests.Helpers;

public static class ServiceCollectionExtensions
{
    public static void ContainsSingletonService<TService, TInstance>(this Mock<IServiceCollection> services) =>
        services.IsRegistered<TService, TInstance>(ServiceLifetime.Singleton);

    private static void IsRegistered<TService, TInstance>(this Mock<IServiceCollection> services, ServiceLifetime lifetime) =>
        services.Verify(serviceCollection => serviceCollection.Add(
            It.Is<ServiceDescriptor>(descriptor => descriptor.Is<TService, TInstance>(lifetime))));
}