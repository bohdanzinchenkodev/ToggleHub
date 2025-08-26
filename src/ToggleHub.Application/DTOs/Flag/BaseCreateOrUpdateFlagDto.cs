using System.Text.Json.Serialization;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Flag;

public abstract class BaseCreateOrUpdateFlagDto
{
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    [JsonPropertyName("returnValueType")]
    public string ReturnValueTypeString { get; set; } = string.Empty;
    [JsonIgnore]
    public ReturnValueType? ReturnValueType => ReturnValueTypeString.ParseEnum<ReturnValueType>();
    public string? DefaultValueOnRaw { get; set; }
    public string? DefaultValueOffRaw { get; set; }
}

public abstract class BaseRuleSetDto
{
    public string? ReturnValueRaw { get; set; }
    public string? OffReturnValueRaw { get; set; }
    public int Priority { get; set; }
    public int Percentage { get; set; } = 100;
}

public abstract class BaseRuleConditionDto
{
    public string Field { get; set; } = string.Empty;
    [JsonPropertyName("fieldType")]
    public string FieldTypeString { get; set; } = string.Empty;
    [JsonIgnore]
    public RuleFieldType? FieldType => FieldTypeString.ParseEnum<RuleFieldType>();
    
    [JsonPropertyName("operator")]
    public string OperatorString { get; set; } = string.Empty;
    [JsonIgnore]
    public OperatorType? Operator => OperatorString.ParseEnum<OperatorType>();
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }
    public bool? ValueBoolean { get; set; }
}

public abstract class BaseRuleConditionItemDto
{
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }
}
