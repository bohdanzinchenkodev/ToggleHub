using FluentValidation;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Helpers;

namespace ToggleHub.Application.Validators.Flag.Base;

public abstract class RuleConditionValidatorBase<T> : AbstractValidator<T> where T : BaseRuleConditionDto
{
    protected RuleConditionValidatorBase()
    {
        RuleFor(x => x.Field)
            .NotEmpty()
            .WithMessage("Field name is required.")
            .MaximumLength(100)
            .WithMessage("Field name must not exceed 100 characters.");

        // Validate that the operator is compatible with the field type
        RuleFor(x => x)
            .Must(x => MustBeValidOperator(x.Operator, x.FieldType))
            .WithMessage(x => WithInvalidOperatorMessage(x.Operator, x.FieldType));

        // Validate value based on field type
        When(x => EnumHelpers.ParseEnum<RuleFieldType>(x.FieldType) == RuleFieldType.String, () =>
        {
            RuleFor(x => x.ValueString)
                .NotEmpty()
                .WithMessage("String value is required for String field type.");
        });

        When(x => EnumHelpers.ParseEnum<RuleFieldType>(x.FieldType) == RuleFieldType.Number, () =>
        {
            RuleFor(x => x.ValueNumber)
                .NotNull()
                .WithMessage("Number value is required for Number field type.");
        });

        When(x => EnumHelpers.ParseEnum<RuleFieldType>(x.FieldType) == RuleFieldType.Boolean, () =>
        {
            RuleFor(x => x.ValueBoolean)
                .NotNull()
                .WithMessage("Boolean value is required for Boolean field type.");
        });
    }
    
    private bool MustBeValidOperator(string operatorType, string fieldType)
    {
        var operatorTypeValue = EnumHelpers.ParseEnum<OperatorType>(operatorType);
        var fieldTypeValue = EnumHelpers.ParseEnum<RuleFieldType>(fieldType);
        return OperatorTypeHelper.IsValidFieldType(operatorTypeValue, fieldTypeValue);
    }
    private string WithInvalidOperatorMessage(string operatorType, string fieldType)
    {
        var fieldTypeValue = EnumHelpers.ParseEnum<RuleFieldType>(fieldType);
        return $"The operator '{operatorType}' is not valid for field type '{fieldType}'. " +
            $"Valid operators for '{fieldType}' are: {string.Join(", ", OperatorTypeHelper.GetValidOperators(fieldTypeValue))}";
    }
    
}
