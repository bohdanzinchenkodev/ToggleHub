namespace ToggleHub.Application.DTOs.Flag.Evaluation;

public class FlagEvaluationResponse
{
    public object? Value { get; set; }
    public string ValueType { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}