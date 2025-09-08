
using Microsoft.Extensions.Caching.Memory;
using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Services;

namespace ToggleHub.Application.UnitTests.Services;

public class FlagEvaluationCacheManagerTests
{
    private FlagEvaluationCacheManager _cacheManager;

    [SetUp]
    public void Setup()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var keyFactory = new FlagEvaluationCacheKeyFactory();
        _cacheManager = new FlagEvaluationCacheManager(memoryCache, keyFactory);
    }

    [Test]
    public async Task SaveAndGet_ShouldBeThreadSafe()
    {
        int orgId = 1, projId = 1, envId = 1;
        string flagKey = "checkout-redesign";

        var contexts = Enumerable.Range(0, 500)
            .Select(i => new FlagEvaluationContext($"user{i}", new Dictionary<string, string?>()))
            .ToList();

        // Save in parallel
        var saveTasks = contexts.Select(ctx => Task.Run(() =>
            _cacheManager.SaveEvaluationResultAsync(
                orgId, projId, envId, flagKey, ctx,
                new FlagEvaluationResult { Value = $"val-{ctx.StickyKey}" })
        ));

        await Task.WhenAll(saveTasks);

        // Retrieve in parallel
        var getTasks = contexts.Select(async ctx =>
        {
            var res = await _cacheManager.GetEvaluationResultAsync(orgId, projId, envId, flagKey, ctx);
            return (ctx.StickyKey, res?.Value);
        });

        var results = await Task.WhenAll(getTasks);

        // Assert all cached results exist and match
        foreach (var (user, val) in results)
        {
            Assert.That(val, Is.EqualTo($"val-{user}"));
        }
    }

    [Test]
    public async Task SaveAndInvalidate_ShouldRemoveAll()
    {
        int orgId = 1, projId = 1, envId = 1;
        string flagKey = "checkout-redesign";

        var contexts = Enumerable.Range(0, 500)
            .Select(i => new FlagEvaluationContext($"user{i}", new Dictionary<string, string?>()))
            .ToArray();

        // Save in parallel
        await Task.WhenAll(contexts.Select(ctx =>
            _cacheManager.SaveEvaluationResultAsync(
                orgId, projId, envId, flagKey, ctx,
                new FlagEvaluationResult { Value = "test" })
        ));

        // Invalidate
        await _cacheManager.InvalidateCacheAsync(orgId, projId, envId, flagKey);

        // Check that none remain
        foreach (var ctx in contexts.Take(20)) // sample check
        {
            var res = await _cacheManager.GetEvaluationResultAsync(orgId, projId, envId, flagKey, ctx);
            Assert.That(res, Is.Null);
        }
    }

}