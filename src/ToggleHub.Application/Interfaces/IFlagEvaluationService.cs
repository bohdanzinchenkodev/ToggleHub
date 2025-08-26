using ToggleHub.Application.DTOs.Flag.Evaluation;

namespace ToggleHub.Application.Interfaces;

public interface IFlagEvaluationService
{
    Task<FlagEvaluationResult> EvaluateAsync(FlagEvaluationRequest request);
}