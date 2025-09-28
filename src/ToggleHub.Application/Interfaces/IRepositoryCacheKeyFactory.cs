using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Interfaces;

public interface IRepositoryCacheKeyFactory
{
    CacheKey ForEntityById<T>(int id) where T : BaseEntity;
    CacheKey ForEntityAll<T>() where T : BaseEntity;
    CacheKey ForSlug<T>(string slug) where T : BaseEntity, ISluggedEntity;
    CacheKey ForSlugPattern<T>(string baseSlug) where T : BaseEntity, ISluggedEntity;
}