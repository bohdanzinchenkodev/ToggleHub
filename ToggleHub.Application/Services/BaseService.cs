using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class BaseService<T> where T : BaseEntity
{
    private readonly IBaseRepository<T> _repository;

    public BaseService(IBaseRepository<T> repository)
    {
        _repository = repository;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        return await _repository.CreateAsync(entity);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        await _repository.UpdateAsync(entity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
