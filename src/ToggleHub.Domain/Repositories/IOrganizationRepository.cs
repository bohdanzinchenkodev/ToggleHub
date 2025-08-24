using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IOrganizationRepository : IBaseRepository<Organization>
{
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsAsync(string name, int excludeId);
}
