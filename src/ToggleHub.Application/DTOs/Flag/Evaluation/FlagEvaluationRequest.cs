namespace ToggleHub.Application.DTOs.Flag.Evaluation;

public class FlagEvaluationRequest
{
    public IDictionary<string, string?> ConditionAttributes { get; set; } = new Dictionary<string, string?>();
    public string UserId { get; set; } = string.Empty;
    public string FlagKey { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public int OrganizationId { get; set; }
}