using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IPagedList<T>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);
    Task<T> CreateAsync(T entity, bool publishEvents = true);
    Task CreateAsync(IEnumerable<T> entities, bool publishEvents = true);
    Task UpdateAsync(T entity, bool publishEvents = true);
    Task UpdateAsync(IEnumerable<T> entities, bool publishEvents = true);
    Task DeleteAsync(int id, bool publishEvents = true);
}
