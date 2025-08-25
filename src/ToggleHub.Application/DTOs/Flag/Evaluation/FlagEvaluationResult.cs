using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Flag.Evaluation;

public class FlagEvaluationResult
{
    public bool Matched { get; set; }
    public ReturnValueType ValueType { get; set; }
    public object? Value { get; set; }
    public int? MatchedRuleSetId { get; set; }
    public string Reason { get; set; } = string.Empty;
}