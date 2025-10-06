using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.API.Middleware;

public class ApiKeyAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyRepository _apiKeyRepo;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyRepository apiKeyRepo)
        : base(options, logger, encoder)  
    {
        _apiKeyRepo = apiKeyRepo;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var header))
            return AuthenticateResult.NoResult();

        if (!header.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.Fail("Invalid Authorization scheme");

        var providedKey = header.ToString()["Bearer ".Length..].Trim();

        var apiKey = await _apiKeyRepo.GetByKeyAsync(providedKey);
        if (apiKey is null)
            return AuthenticateResult.Fail("Invalid or expired API key");

        apiKey.LastUsedAt = DateTime.UtcNow; 
        await _apiKeyRepo.UpdateAsync(apiKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, apiKey.OrganizationId.ToString()),
            new Claim("OrgId", apiKey.OrganizationId.ToString()),
            new Claim("ProjectId", apiKey.ProjectId.ToString()),
            new Claim("EnvironmentId", apiKey.EnvironmentId.ToString()),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}