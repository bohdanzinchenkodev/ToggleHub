using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Infrastructure.Cache;

public class FlagEvaluationCacheManager : IFlagEvaluationCacheManager
{
    private readonly IMemoryCache _memoryCache;
    private readonly IFlagEvaluationCacheKeyFactory _cacheKeyFactory;
    // Index to track cache keys for each flag
    private static readonly ConcurrentDictionary<string, HashSet<string>> Index = new();
    
    public FlagEvaluationCacheManager(IMemoryCache memoryCache, IFlagEvaluationCacheKeyFactory cacheKeyFactory)
    {
        _memoryCache = memoryCache;
        _cacheKeyFactory = cacheKeyFactory;
    }

    public Task SaveEvaluationResultAsync(
        int organizationId,
        int projectId,
        int environmentId,
        string flagKey,
        FlagEvaluationContext context,
        FlagEvaluationResult result)
    {
        var cacheKey = _cacheKeyFactory.CreateCacheKey(organizationId, projectId, environmentId, flagKey, context);
        _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        // Update index
        var indexKey = GetIndexKey(organizationId, projectId, environmentId, flagKey);
        var set = Index.GetOrAdd(indexKey, _ => []);
        lock (set)
        { 
            set.Add(cacheKey);
        }
        return Task.CompletedTask;
    }

    public Task<FlagEvaluationResult?> GetEvaluationResultAsync(int organizationId, int projectId, int environmentId, string flagKey, FlagEvaluationContext context)
    {
        var cacheKey = _cacheKeyFactory.CreateCacheKey(organizationId, projectId, environmentId, flagKey, context);
        _memoryCache.TryGetValue(cacheKey, out FlagEvaluationResult? result);
        return Task.FromResult(result);
    }

    public Task InvalidateCacheAsync(int organizationId, int projectId, int environmentId, string flagKey)
    {
        var indexKey = GetIndexKey(organizationId, projectId, environmentId, flagKey);
        if (!Index.TryRemove(indexKey, out var set))
            return Task.CompletedTask;

        lock (set)
        {
            foreach (var cacheKey in set)
            {
                _memoryCache.Remove(cacheKey);
            }
        }

        return Task.CompletedTask;
    }

    private string GetIndexKey(int organizationId, int projectId, int environmentId, string flagKey)
    {
        return $"{organizationId}:{projectId}:{environmentId}:{flagKey}";
    }
    
    
}