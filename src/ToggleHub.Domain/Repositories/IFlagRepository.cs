using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IFlagRepository : IBaseRepository<Flag>
{
    Task<bool> ExistsAsync(string key, int environmentId, int projectId);
    Task<Flag?> GetFlagByKeyAsync(string key, int environmentId, int projectId);
}
