using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Application.Services;

public class FlagEvaluationCacheKeyFactory : IFlagEvaluationCacheKeyFactory
{
    public string CreateCacheKey(
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

        // Compose final cache key
        return $"flag-eval:{organizationId}:{projectId}:{environmentId}:{flagKey}:{userId}:{hash}";
    }
}