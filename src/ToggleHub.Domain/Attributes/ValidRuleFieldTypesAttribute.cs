using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ValidRuleFieldTypesAttribute : Attribute
{
    public RuleFieldType[] ValidFieldTypes { get; }

    public ValidRuleFieldTypesAttribute(params RuleFieldType[] validFieldTypes)
    {
        ValidFieldTypes = validFieldTypes;
    }
}
