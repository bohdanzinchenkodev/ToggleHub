namespace ToggleHub.Domain.Entities;

public class Organization : BaseEntity, ISluggedEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}