namespace ToggleHub.Domain.Entities;

public class OrganizationInvite : BaseEntity
{
    
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedByUserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public InviteStatus Status { get; set; } = InviteStatus.Pending;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? DeclinedAt { get; set; }
}
public enum InviteStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2,
    Expired = 3,
    Revoked = 4
}