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
        
        RuleFor(x => x.FieldType)
            .NotNull()
            .WithMessage("Field type is invalid.");
        
        RuleFor(x => x.Operator)
            .NotNull()
            .WithMessage("Operator is invalid.");

        // Validate that the operator is compatible with the field type
        When(x => x.FieldType.HasValue && x.Operator.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(dto => MustBeValidOperator(dto.Operator, dto.FieldType))
                .WithMessage(WithInvalidOperatorMessage);
        });

        
        // Validate value based on field type
        When(x => x.FieldType == RuleFieldType.String, () =>
        {
            RuleFor(x => x.ValueString)
                .NotEmpty()
                .WithMessage("String value is required for String field type.");
        });

        When(x => x.FieldType == RuleFieldType.Number, () =>
        {
            RuleFor(x => x.ValueNumber)
                .NotNull()
                .WithMessage("Number value is required for Number field type.");
        });

        When(x => x.FieldType == RuleFieldType.Boolean, () =>
        {
            RuleFor(x => x.ValueBoolean)
                .NotNull()
                .WithMessage("Boolean value is required for Boolean field type.");
        });
    }
    
    private bool MustBeValidOperator(OperatorType? operatorType, RuleFieldType? fieldType)
    {
        
        return OperatorTypeHelper.IsValidFieldType(operatorType!.Value, fieldType!.Value);
    }
    private string WithInvalidOperatorMessage(BaseRuleConditionDto dto)
    {
        var fieldType = dto.FieldType!;
        var operatorString = dto.OperatorString;
        var fieldTypeString = dto.FieldTypeString;
        
        return $"The operator '{operatorString}' is not valid for field type '{fieldTypeString}'. " +
            $"Valid operators for '{fieldTypeString}' are: {string.Join(", ", OperatorTypeHelper.GetValidOperators(fieldType.Value))}";
    }
    
}
