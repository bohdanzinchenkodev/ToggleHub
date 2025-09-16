using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IOrganizationRepository : IBaseSluggedRepository<Organization>
{
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsAsync(string name, int excludeId);
    Task<IPagedList<Organization>> GetOrganizationsByUserIdAsync(int userId, int pageIndex = 0, int pageSize = int.MaxValue);
}
