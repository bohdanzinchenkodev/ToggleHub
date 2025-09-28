using ToggleHub.Application;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Infrastructure.Settings;

namespace ToggleHub.Infrastructure.Cache;

public class RepositoryCacheKeyFactory : IRepositoryCacheKeyFactory
{
    private readonly ICacheKeyFormatter _keyFormatter;
    private readonly CacheSettings _settings;

    public RepositoryCacheKeyFactory(
        ICacheKeyFormatter keyFormatter,
        CacheSettings settings)
    {
        _keyFormatter = keyFormatter;
        _settings = settings;
    }

    public CacheKey ForEntityById<T>(int id) where T : BaseEntity
    {
        var key = _keyFormatter.Format("entity:{0}:id:{1}", typeof(T).Name.ToLower(), id);
        return new CacheKey(key, GetCacheTime<T>());
    }

    public CacheKey ForEntityAll<T>(int page, int pageSize) where T : BaseEntity
    {
        var key = _keyFormatter.Format("entity:{0}:page:{1}:{2}", typeof(T).Name.ToLower(), page, pageSize);
        return new CacheKey(key, GetCacheTime<T>());
    }

    public CacheKey ForSlug<T>(string slug) where T : BaseEntity, ISluggedEntity
    {
        var key = _keyFormatter.Format("entity:{0}:slug:{1}", typeof(T).Name.ToLower(), slug);
        return new CacheKey(key, GetCacheTime<T>());
    }

    public CacheKey ForSlugPattern<T>(string baseSlug) where T : BaseEntity, ISluggedEntity
    {
        var key = _keyFormatter.Format("entity:{0}:slugpattern:{1}", typeof(T).Name.ToLower(), baseSlug);
        return new CacheKey(key, GetCacheTime<T>());
    }

    public string PrefixForEntity<T>() where T : BaseEntity
    {
        return $"entity:{typeof(T).Name.ToLower()}:";
    }

    private int GetCacheTime<T>()
    {
        return _settings.DefaultCacheTimeMinutes;
    }
}
