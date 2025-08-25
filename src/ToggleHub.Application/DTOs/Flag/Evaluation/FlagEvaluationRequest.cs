namespace ToggleHub.Application.DTOs.Flag.Evaluation;

public class FlagEvaluationRequest
{
    public IDictionary<string, object> ConditionAttributes { get; set; } = new Dictionary<string, object>();
    public string? UserId { get; set; }
    public string FlagKey { get; set; } = string.Empty;
    public string ProjectSlug { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string OrganizationSlug { get; set; } = string.Empty;
}