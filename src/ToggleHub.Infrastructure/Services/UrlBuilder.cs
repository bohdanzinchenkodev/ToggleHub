using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Settings;

namespace ToggleHub.Infrastructure.Services;

public class UrlBuilder : IUrlBuilder
{
    private readonly ApplicationUrlSettings _urlSettings;

    public UrlBuilder(ApplicationUrlSettings urlSettings)
    {
        _urlSettings = urlSettings;
    }

    public string BuildPasswordResetUrl(string token, string email)
    {
        var encodedToken = Uri.EscapeDataString(token);
        var encodedEmail = Uri.EscapeDataString(email);
        
        return $"{_urlSettings.ClientBaseUrl.TrimEnd('/')}/reset-password?token={encodedToken}&email={encodedEmail}";
    }

    public string BuildOrganizationInviteAcceptUrl(int organizationId, string token)
    {
        return $"{_urlSettings.ClientBaseUrl.TrimEnd('/')}/organizations/{organizationId}/invites/accept/{token}";
    }

    public string BuildOrganizationInviteDeclineUrl(int organizationId, string token)
    {
        return $"{_urlSettings.ClientBaseUrl.TrimEnd('/')}/organizations/{organizationId}/invites/decline/{token}";
    }
}
