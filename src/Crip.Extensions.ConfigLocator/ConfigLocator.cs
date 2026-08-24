using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Crip.Extensions.ConfigLocator.DependencyInjection;
using Crip.Extensions.ConfigLocator.Generics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Crip.Extensions.ConfigLocator;

/// <summary>
/// Configuration locator service extensions.
/// </summary>
public static class ConfigLocator
{
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Register all options classes with <see cref="ConfigLocationAttribute"/>
        /// attribute from the calling assembly.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> to read from.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public IServiceCollection AddConfigLocator(IConfiguration configuration) =>
            services.AddConfigLocator(configuration, [Assembly.GetCallingAssembly()]);

        /// <summary>
        /// Register all options classes with <see cref="ConfigLocationAttribute"/>
        /// attribute from provided <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> to read from.</param>
        /// <param name="assemblies">
        /// The collection of <see cref="Assembly"/> where search for classes with
        /// <see cref="ConfigLocationAttribute"/> attribute.
        /// </param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddConfigLocator(IConfiguration configuration, params Assembly[]? assemblies)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            services.AddOptions();

            foreach (var type in assemblies.GetDefinedTypes().Distinct())
            {
                var locations = type.GetCustomAttributes<ConfigLocationAttribute>().ToArray();
                foreach (var location in locations)
                {
                    services.AddConfigurationOf(configuration, type, location);
                }

                var validate = type.GetCustomAttribute<ConfigValidateAttribute>();
                if (validate is null) continue;

                var names = locations.Select(location => location.Name).DefaultIfEmpty(Options.DefaultName).ToArray();
                services.AddDataAnnotationValidateOptions(type, names);
                services.AddCustomValidateOf(type, validate);
            }

            return services;
        }

        private void AddConfigurationOf(IConfiguration configuration, Type type, ConfigLocationAttribute attribute)
        {
            var section = configuration.GetSection(attribute.SectionKey);
            var types = type.WithAdditionalTypesOf(attribute).Distinct().ToArray();

            services.Configure(section, attribute.Name, types);
        }

        private void AddCustomValidateOf(Type type, ConfigValidateAttribute attribute)
        {
            services.AddCustomValidateOptions(type, attribute.Validators);
        }
    }

    private static IEnumerable<Type> WithAdditionalTypesOf(this Type type, ConfigLocationAttribute attribute)
    {
        yield return type;

        foreach (var additionalType in attribute.AdditionalTypes)
        {
            yield return additionalType;
        }
    }
}