using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IProjectRepository : IBaseRepository<Project>
{
    Task<bool> NameExistsAsync(string name, int organizationId = 0);
    Task<IEnumerable<Project>> GetAllAsync(int? organizationId = null);
}
