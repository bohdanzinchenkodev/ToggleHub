using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ToggleHub.Application;
using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Infrastructure.Cache;

public class FlagEvaluationCacheKeyFactory : IFlagEvaluationCacheKeyFactory
{
    private readonly ICacheKeyFactory _cacheKeyFactory;

    public FlagEvaluationCacheKeyFactory(ICacheKeyFactory cacheKeyFactory)
    {
        _cacheKeyFactory = cacheKeyFactory;
    }

    public CacheKey CreateCacheKey(
        int organizationId,
        int projectId,
        int environmentId,
        string flagKey,
        FlagEvaluationContext context)
    {
        // Ensure context is never null
        var userId = context.StickyKey ?? "anonymous";

        // Serialize condition attributes (sorted for determinism)
        var normalizedAttributes = context.Attrs?
                                       .OrderBy(kvp => kvp.Key)
                                       .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                                   ?? new Dictionary<string, string?>();

        var json = JsonSerializer.Serialize(normalizedAttributes);

        // Hash the JSON so the cache key stays short
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        var hash = Convert.ToHexString(hashBytes);

        var keyParts = new Dictionary<string, object?>
        {
            {"operation", "flag_evaluation"},
            { nameof(organizationId), organizationId },
            { nameof(projectId), projectId },
            { nameof(environmentId), environmentId },
            { nameof(flagKey), flagKey },
            { nameof(userId), userId },
            { "attrsHash", hash }
        };
        return _cacheKeyFactory.For<Flag>(keyParts);
    }
}