using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ToggleHub.Application;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Infrastructure.Cache;

public class InMemoryCacheManager : ICacheManager
{
    private readonly IMemoryCache _memoryCache;
    private readonly ICacheKeyRegistry _keyRegistry;
    private CancellationTokenSource _clearToken = new();

    public InMemoryCacheManager(IMemoryCache memoryCache, ICacheKeyRegistry keyRegistry)
    {
        _memoryCache = memoryCache;
        _keyRegistry = keyRegistry;
    }

    public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
    {
        if (key.CacheTime <= 0)
            return await acquire();

        var lazy = _memoryCache.GetOrCreate(
            key.Key,
            entry =>
            {
                entry.SetOptions(PrepareEntryOptions(key));
                // concurrency-safe lazy init
                return new Lazy<Task<T>>(acquire, true);
            });

        try
        {
            var result = await lazy!.Value;
            if (result == null)
                await RemoveAsync(key.Key);

            return result;
        }
        catch
        {
            await RemoveAsync(key.Key);
            throw;
        }
    }

    public Task SetAsync<T>(CacheKey key, T data)
    {
        if (data != null && key.CacheTime > 0)
        {
            _memoryCache.Set(
                key.Key,
                new Lazy<Task<T>>(() => Task.FromResult(data), true),
                PrepareEntryOptions(key));
        }
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        _keyRegistry.RemoveKey(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix)
    {
        var keys = _keyRegistry.GetKeys().Where(k => k.StartsWith(prefix));
        foreach (var key in keys)
        {
            _memoryCache.Remove(key);
            _keyRegistry.RemoveKey(key);
        }
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _clearToken.Cancel();// expire everything with the old token
        _clearToken.Dispose();
        _clearToken = new CancellationTokenSource();

        _keyRegistry.Clear();// reset tracked keys
        return Task.CompletedTask;
    }

    // prepare expiration + tracking
    protected virtual MemoryCacheEntryOptions PrepareEntryOptions(CacheKey key)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
        };

        // token allows global clear
        options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));
        options.RegisterPostEvictionCallback(OnEviction);

        _keyRegistry.AddKey(key.Key);
        return options;
    }

    protected virtual void OnEviction(object evictedKey, object? value, EvictionReason reason, object? state)
    {
        if (evictedKey is not string key) return;

        switch (reason)
        {
            case EvictionReason.Removed:
            case EvictionReason.Replaced:
            case EvictionReason.TokenExpired:
                // handled elsewhere
                break;
            default:
                _keyRegistry.RemoveKey(key);
                break;
        }
    }
}