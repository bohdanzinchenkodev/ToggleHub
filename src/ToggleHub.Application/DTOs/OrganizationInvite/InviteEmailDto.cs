namespace ToggleHub.Application.DTOs.OrganizationInvite;

public class InviteEmailDto
{
    public string OrganizationName { get; set; } = "";
    public string InviteLink { get; set; } = "";
    public string DeclineLink { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
}