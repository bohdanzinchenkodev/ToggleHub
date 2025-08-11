using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface ISluggedRepository<T> where T : ISluggedEntity
{
    Task<T?> GetBySlugAsync(string slug); 
    Task<IEnumerable<string>> GetSlugsByPatternAsync(string pattern);
    
}