using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using ToggleHub.API.Middleware;
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
}