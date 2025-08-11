using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IOrganizationRepository : IBaseRepository<Organization>
{
    Task<IEnumerable<string>> GetSlugsByPatternAsync(string baseSlug);
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsAsync(string name, int excludeId);
}
