namespace ToggleHub.Domain.Entities;

public enum OrgMemberRole
{
    Owner = 10,
    Admin = 20,
    FlagManager = 30
}

public class OrgMember : BaseEntity
{
    public int OrgId { get; set; }
    public int UserId { get; set; }
    public OrgMemberRole Role { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
}
