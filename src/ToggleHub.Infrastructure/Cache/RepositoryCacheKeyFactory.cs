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

    public CacheKey For<T>(Dictionary<string, object?> parameters) where T : BaseEntity
    {
        // always start with entity type as the base part
        var parts = new List<string> { "entity:{0}" };
        var values = new List<object> { typeof(T).Name.ToLower() };

        // append dictionary keys in stable order
        foreach (var kvp in parameters.OrderBy(p => p.Key))
        {
            parts.Add($"{kvp.Key}={{{values.Count}}}");
            values.Add(kvp.Value ?? "null");
        }

        var template = string.Join(":", parts);
        var formatted = _keyFormatter.Format(template, values.ToArray());

        return new CacheKey(formatted, GetCacheTime<T>());
    }

    public CacheKey ForEntityById<T>(int id) where T : BaseEntity
    {
        return For<T>(new Dictionary<string, object?>
        {
            { nameof(id), id }
        });
    }

    public CacheKey ForEntityAll<T>(int page, int pageSize) where T : BaseEntity
    {
        return For<T>(new Dictionary<string, object?>
        {
            { nameof(page), page },
            { nameof(pageSize), pageSize }
        });
    }

    public CacheKey ForSlug<T>(string slug) where T : BaseEntity, ISluggedEntity
    {
        return For<T>(new Dictionary<string, object?>
        {
            { nameof(slug), slug }
        });
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
