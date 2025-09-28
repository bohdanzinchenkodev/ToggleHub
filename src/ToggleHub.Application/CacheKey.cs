namespace ToggleHub.Application;

public class CacheKey(string key, int cacheTime)
{
    public string Key { get; } = key;
    public int CacheTime { get; } = cacheTime;
}