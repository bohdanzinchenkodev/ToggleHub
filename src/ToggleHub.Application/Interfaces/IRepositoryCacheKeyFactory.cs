using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Interfaces;

public interface IRepositoryCacheKeyFactory
{
    CacheKey For<T>(Dictionary<string, object?> parameters) where T : BaseEntity;
    CacheKey ForEntityById<T>(int id) where T : BaseEntity;
    CacheKey ForEntityAll<T>(int page, int pageSize) where T : BaseEntity;
    CacheKey ForSlug<T>(string slug) where T : BaseEntity, ISluggedEntity;
    string PrefixForEntity<T>() where T : BaseEntity;
}