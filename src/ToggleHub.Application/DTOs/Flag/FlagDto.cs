using System.Text.Json.Serialization;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Flag;

public class FlagDto
{
    public int Id { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int ProjectId { get; set; }
    public int EnvironmentId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    [JsonPropertyName("returnValueType")]
    public string ReturnValueTypeString => ReturnValueType.ParseEnumToString(); 
    [JsonIgnore]
    public ReturnValueType ReturnValueType { get; set; }
    public string? DefaultValueOnRaw { get; set; }
    public string? DefaultValueOffRaw { get; set; }
    public IList<RuleSetDto> RuleSets { get; set; } = new List<RuleSetDto>();
}

public class RuleSetDto
{
    public int Id { get; set; }
    public string? ReturnValueRaw { get; set; }
    public string? OffReturnValueRaw { get; set; }
    public int Priority { get; set; }
    public int Percentage { get; set; } = 100;
    public IList<RuleConditionDto> Conditions { get; set; } = new List<RuleConditionDto>();
}

public class RuleConditionDto
{
    public int Id { get; set; }
    public string Field { get; set; } = string.Empty;
    [JsonPropertyName("fieldType")]
    public string FieldTypeString => FieldType.ParseEnumToString();
    [JsonIgnore]
    public RuleFieldType FieldType { get; set; }

    [JsonPropertyName("operator")]
    public string OperatorString => Operator.ParseEnumToString();
    [JsonIgnore]
    public OperatorType Operator { get; set; }
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }
    public bool? ValueBoolean { get; set; }
    public IList<RuleConditionItemDto> Items { get; set; } = new List<RuleConditionItemDto>();
}

public class RuleConditionItemDto
{
    public int Id { get; set; }
    public string? ValueString { get; set; }
    public decimal? ValueNumber { get; set; }
}
