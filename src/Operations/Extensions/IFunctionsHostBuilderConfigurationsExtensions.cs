using System;
using System.Linq;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Operations.Extensions
{
    internal static class FunctionsHostBuilderConfigurationsExtensions
    {
        public static IFunctionsHostBuilder AddConfiguration(this IFunctionsHostBuilder builder, Action<IConfigurationBuilder> configBuilderFunc = null)
        {
            var services = builder.Services;

            var configBuilder = new ConfigurationBuilder().SetBasePath(GetCurrentDirectory());
            configBuilderFunc?.Invoke(configBuilder);

            var newConfig = configBuilder.Build();

            var descriptor = services.First(sd => sd.ServiceType == typeof(IConfiguration));
            if (descriptor.ImplementationInstance is IConfigurationRoot configuration)
            {
                var providers = configuration.Providers
                    .Concat(newConfig.Providers)
                    .ToList();

                services.Replace(ServiceDescriptor.Singleton<IConfiguration>(new ConfigurationRoot(providers)));
            }
            else if (descriptor.ImplementationFactory != null)
            {
                var configFactory = descriptor.ImplementationFactory;

                services.Replace(ServiceDescriptor.Singleton<IConfiguration>(sp =>
                {
                    var configuration = (IConfigurationRoot)configFactory.Invoke(sp);
                    var providers = configuration.Providers
                        .Concat(newConfig.Providers)
                        .ToList();

                    return new ConfigurationRoot(providers);
                }));
            }

            return builder;
        }

        private static string GetCurrentDirectory()
        {
            return Environment.GetEnvironmentVariable("FunctionsAppDirectory") ?? Environment.CurrentDirectory;
        }
    }
}
