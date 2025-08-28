using ToggleHub.Domain.Entities;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Domain.Repositories;

public interface IEnvironmentRepository : IBaseRepository<Environment>
{
    Task<IEnumerable<Environment>> GetAllAsync(int? projectId = null);
    Task<bool> EnvironmentExistsAsync(EnvironmentType type, int projectId);
}
