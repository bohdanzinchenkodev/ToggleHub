using System.Collections.Concurrent;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Infrastructure.Cache;
 
public class InMemoryCacheKeyRegistry : ICacheKeyRegistry

{
    private readonly ConcurrentDictionary<string, byte> _keys = new();

    public void AddKey(string key) => _keys.TryAdd(key, 0);
    public void RemoveKey(string key) => _keys.TryRemove(key, out _);
    public IReadOnlyCollection<string> GetKeys() => _keys.Keys.ToList();
    public void Clear() => _keys.Clear();
}