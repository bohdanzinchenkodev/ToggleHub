namespace ToggleHub.Application.Interfaces;

public interface IUrlBuilder
{
    string BuildPasswordResetUrl(string token, string email);
    string BuildOrganizationInviteAcceptUrl(int organizationId, string token);
    string BuildOrganizationInviteDeclineUrl(int organizationId, string token);
}
