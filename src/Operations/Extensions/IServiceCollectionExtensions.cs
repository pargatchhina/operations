using System;
using Operations.Infrastructure.Services;
using Operations.Shared.Options;
using MediatR;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Operations.Infrastructure.Handlers;

namespace Operations.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.ConfigureMessageSender();
            services.ConfigureOrderService();
            services.ConfigureMediator();

            return services;
        }

        public static void ConfigureMessageSender(this IServiceCollection services)
        {
            services.AddScoped<IMessageSender>(x =>
            {
                var messagingProvider = x.GetService<MessagingProvider>();

                var topic = Environment.GetEnvironmentVariable("topicName");
                var connectionString = Environment.GetEnvironmentVariable("SchedulerServiceBusConnection");

                return messagingProvider.CreateMessageSender($"{topic}", connectionString);
            });
        }

        public static void ConfigureOrderService(this IServiceCollection services)
        {
            services.AddHttpClient("orderService")
                .OrderServiceClient()
                .AddTypedClient<IOrderService, OrderService>();

            services.AddHttpClient("paymentService")
                .PaymentServiceClient()
                .AddTypedClient<IPaymentService, PaymentService>();
        }

        public static IHttpClientBuilder OrderServiceClient(this IHttpClientBuilder httpClientBuilder)
        {
            return httpClientBuilder.ConfigureHttpClient((provider, httpClient) =>
            {
                var options = provider.GetRequiredService<IOptions<OrderServiceOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("api-key", options.ApiKey);
            });
        }
        public static IHttpClientBuilder PaymentServiceClient(this IHttpClientBuilder httpClientBuilder)
        {
            return httpClientBuilder.ConfigureHttpClient((provider, httpClient) =>
            {
                var options = provider.GetRequiredService<IOptions<PaymentServiceOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }

        public static void ConfigureMediator(this IServiceCollection services)
        {
            services.AddMediatR(typeof(OrderCancelScheduledHandler).Assembly);
        }

        public static IConfigurationBuilder AddKeyVault(this IConfigurationBuilder configurationBuilder)
        {
            var configuration = configurationBuilder.Build();

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider.KeyVaultTokenCallback));

            configurationBuilder.AddAzureKeyVault(configuration["KeyVault:Url"],
                keyVaultClient, new DefaultKeyVaultSecretManager());

            return configurationBuilder;
        }

        public static void BindConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OrderServiceOptions>(options =>
                configuration.GetSection("OrderService")
                    .Bind(options));

            services.Configure<PaymentServiceOptions>(options =>
                configuration.GetSection("PaymentService")
                    .Bind(options));
        }
    }
}