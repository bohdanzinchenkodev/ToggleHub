using System;
using Microsoft.Extensions.DependencyInjection;
using ToggleHub.Sdk.Clients;
using ToggleHub.Sdk.Options;

namespace ToggleHub.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a typed HttpClient + IFlagsClient.
        /// </summary>
        public static IServiceCollection AddToggleHubClient(
            this IServiceCollection services,
            Action<ToggleHubClientOptions> configure)
        {   
            var opts = new ToggleHubClientOptions();
            configure(opts);
            if (string.IsNullOrWhiteSpace(opts.ApiKey))
                throw new ArgumentException("ApiKey must be provided.", nameof(configure));
            
            services.AddSingleton(opts);

            services.AddHttpClient<IFlagsClient, FlagsClient>(client =>
            {
                client.BaseAddress = new Uri(opts.BaseAddress, UriKind.Absolute);
                client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
            });

            return services;
        }
    }
}