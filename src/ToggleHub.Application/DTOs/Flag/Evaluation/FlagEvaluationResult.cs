using System.Text.Json.Serialization;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Flag.Evaluation;

public class FlagEvaluationResult
{
    public bool Matched { get; set; }
    [JsonIgnore]
    public ReturnValueType ValueType { get; set; }
    [JsonPropertyName("valueType")]
    public string ValueTypeString => ValueType.ParseEnumToString();
    public object? Value { get; set; }
    public int? MatchedRuleSetId { get; set; }
    public string Reason { get; set; } = string.Empty;
}