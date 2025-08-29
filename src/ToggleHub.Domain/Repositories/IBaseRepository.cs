using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IPagedList<T>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);
    Task<T> CreateAsync(T entity);
    Task CreateAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateAsync(IEnumerable<T> entities);
    Task DeleteAsync(int id);
}
