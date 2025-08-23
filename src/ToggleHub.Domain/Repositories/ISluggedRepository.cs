using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface ISluggedRepository 
{
    Task<T?> GetBySlugAsync<T>(string slug) where T : BaseEntity, ISluggedEntity; 
    Task<IEnumerable<string>> GetSlugsByPatternAsync<T>(string pattern) where T : BaseEntity, ISluggedEntity;
    
}