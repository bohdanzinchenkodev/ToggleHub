namespace ToggleHub.Domain.Entities;

public enum OrgMemberRole
{
    Owner,
    Admin,
    Editor,
    Viewer
}

public class OrgMember : BaseEntity
{
    public int OrgId { get; set; }
    public int UserId { get; set; }
    public OrgMemberRole Role { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public User User { get; set; } = null!;
}
