namespace ToggleHub.Domain.Entities;

public class Project : BaseEntity, ISluggedEntity
{
    public int OrgId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public Organization Organization { get; set; } = null!;
}
