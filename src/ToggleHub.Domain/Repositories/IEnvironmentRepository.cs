using ToggleHub.Domain.Entities;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Domain.Repositories;

public interface IEnvironmentRepository : IBaseRepository<Environment>
{
    Task<IEnumerable<Environment>> GetAllAsync(int? organizationId = null);
}
