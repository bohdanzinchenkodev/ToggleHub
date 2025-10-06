using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IBaseSluggedRepository<T> : IBaseRepository<T> where T : BaseEntity, ISluggedEntity
{
    Task<T?> GetBySlugAsync(string slug);
    Task<IEnumerable<string>> GetSlugsByPatternAsync(string baseSlug);
}
