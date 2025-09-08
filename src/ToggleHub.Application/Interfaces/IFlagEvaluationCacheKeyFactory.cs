using ToggleHub.Application.DTOs.Flag.Evaluation;

namespace ToggleHub.Application.Interfaces;

public interface IFlagEvaluationCacheKeyFactory
{
    string CreateCacheKey(int organizationId, int projectId, int environmentId, string flagKey, FlagEvaluationContext context);
}