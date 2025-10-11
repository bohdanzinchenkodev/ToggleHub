using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ToggleHub.API.Middleware;
using ToggleHub.API.Settings;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.API.Extensions;

public static class ServiceCollectionExtensions 
{
    public static IServiceCollection AddApiKeyAuth(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                AuthConstants.AuthSchemes.ApiKey, null);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthConstants.AuthPolicies.RequireIdentity, policy =>
                policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                    .RequireAuthenticatedUser());

            options.AddPolicy(AuthConstants.AuthPolicies.RequireApiKey, policy =>
                policy.AddAuthenticationSchemes(AuthConstants.AuthSchemes.ApiKey)
                    .RequireAuthenticatedUser());
        });

        return services;
    }
    
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration, ILoggingBuilder loggingBuilder)
    {
        var openTelemetrySettings = configuration.GetSection("OpenTelemetry").Get<OpenTelemetrySettings>();
        if (openTelemetrySettings is null)
            return services;
        
        services.AddSingleton(openTelemetrySettings);
        if (!openTelemetrySettings.Enabled || 
            openTelemetrySettings is { OtlpLogsEnabled: false, OtlpMetricsEnabled: false, OtlpTracesEnabled: false })
            return services;
        
        loggingBuilder.ClearProviders();
        loggingBuilder.AddOpenTelemetry(o =>
        {
            o.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(openTelemetrySettings.ServiceName, serviceVersion: openTelemetrySettings.ServiceVersion));
            o.AddConsoleExporter();
            o.IncludeFormattedMessage = true;
            o.IncludeScopes = true;
            o.ParseStateValues = true;
            if(openTelemetrySettings.OtlpLogsEnabled)
                o.AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(openTelemetrySettings.OtlpEndpointLogs);
                    opt.Protocol = openTelemetrySettings.GetProtocol();
                });
        });
        var otlpBuilder = services.AddOpenTelemetry()
            .ConfigureResource(x => x.AddService(openTelemetrySettings.ServiceName, serviceVersion: openTelemetrySettings.ServiceVersion));
        if (openTelemetrySettings.OtlpMetricsEnabled)
        {
            otlpBuilder.WithMetrics(mb =>
            {
                // Useful built-ins
                mb.AddAspNetCoreInstrumentation(); // request duration, active reqs, etc.
                mb.AddHttpClientInstrumentation(); // outgoing HTTP
                mb.AddRuntimeInstrumentation(); // GC, LOH, exceptions, locks
                mb.AddProcessInstrumentation(); // CPU, mem, threads (optional)

                mb.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(openTelemetrySettings.OtlpEndpointMetrics);
                    o.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
            });
        }

        if (openTelemetrySettings.OtlpTracesEnabled)
        {
            otlpBuilder.WithTracing(tb =>
            {
                tb.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(openTelemetrySettings.OtlpEndpointMetrics);
                        o.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            });
        }



        return services;
    }
}