namespace ToggleHub.Application.Interfaces;

public interface ICacheManager
{
    Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire);
    Task SetAsync<T>(CacheKey key, T data);
    Task RemoveByPrefixAsync(string prefix);
    Task RemoveAsync(string key);
    Task ClearAsync();
}