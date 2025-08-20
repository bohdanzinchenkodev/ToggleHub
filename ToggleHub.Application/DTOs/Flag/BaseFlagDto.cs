using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Flag;

public abstract class BaseFlagDto
{
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; }
}

public abstract class BaseRuleSetDto
{
    public string? ReturnValueRaw { get; set; }
    public string? OffReturnValueRaw { get; set; }
    public string ReturnValueType { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int Percentage { get; set; } = 100;
}

public abstract class BaseRuleConditionDto
{
    public string Field { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }
    public bool? ValueBoolean { get; set; }
}

public abstract class BaseRuleConditionItemDto
{
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }
}
