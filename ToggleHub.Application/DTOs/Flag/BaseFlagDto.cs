using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Flag;

public abstract class BaseFlagDto
{
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public required string Key { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
}

public abstract class BaseRuleSetDto
{
    public string? ReturnValueRaw { get; set; }
    public string? OffReturnValueRaw { get; set; }
    public ReturnValueType ReturnValueType { get; set; } = ReturnValueType.Boolean;
    public int Priority { get; set; }
    public int Percentage { get; set; } = 100;
}

public abstract class BaseRuleConditionDto
{
    public required string Field { get; set; }
    public RuleFieldType FieldType { get; set; }
    public OperatorType Operator { get; set; }
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }
    public bool? ValueBoolean { get; set; }
}

public abstract class BaseRuleConditionItemDto
{
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }
}
