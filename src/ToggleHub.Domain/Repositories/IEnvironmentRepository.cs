using ToggleHub.Domain.Entities;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Domain.Repositories;

public interface IEnvironmentRepository : IBaseRepository<Environment>
{
    Task<IPagedList<Environment>> GetAllAsync(int? projectId = null, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<bool> EnvironmentExistsAsync(EnvironmentType type, int projectId);
}
