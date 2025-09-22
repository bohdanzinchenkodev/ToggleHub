namespace ToggleHub.Application.DTOs.OrganizationInvite;

public class CreateOrganizationInviteDto
{
    public int OrganizationId { get; set; }
    public string Email { get; set; } = null!;
}
