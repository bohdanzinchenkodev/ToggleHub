namespace ToggleHub.Application.DTOs.OrganizationInvite;

public class AcceptInviteDto
{
    public string Token { get; set; } = null!;
    public int UserId { get; set; }
}
