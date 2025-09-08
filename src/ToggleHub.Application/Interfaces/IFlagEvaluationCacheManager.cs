using ToggleHub.Application.DTOs.Flag.Evaluation;

namespace ToggleHub.Application.Interfaces;

public interface IFlagEvaluationCacheManager
{
    Task SaveEvaluationResultAsync(int organizationId, int projectId, int environmentId, string flagKey, FlagEvaluationContext context, FlagEvaluationResult result);
    Task<FlagEvaluationResult?> GetEvaluationResultAsync(int organizationId, int projectId, int environmentId, string flagKey, FlagEvaluationContext context);
    Task InvalidateCacheAsync(int organizationId, int projectId, int environmentId, string flagKey);
}