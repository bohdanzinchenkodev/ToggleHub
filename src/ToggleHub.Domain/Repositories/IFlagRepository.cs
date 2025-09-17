using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IFlagRepository : IBaseRepository<Flag>
{
    Task<bool> ExistsAsync(string key, int environmentId, int projectId);
    Task<Flag?> GetFlagByKeyAsync(string key, int environmentId, int projectId);
    Task<IPagedList<Flag>> GetAllAsync(int? projectId = null, int? environmentId = null, int pageIndex = 0, int pageSize = int.MaxValue);
}
