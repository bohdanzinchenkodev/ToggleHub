using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IProjectRepository : IBaseRepository<Project>
{
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsAsync(string name, int excludeId);
    Task<IEnumerable<Project>> GetAllAsync(int? organizationId = null);
}
