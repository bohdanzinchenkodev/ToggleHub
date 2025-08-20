using FluentValidation;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Domain.Entities;

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
            .IsInEnum()
            .WithMessage("Invalid field type.");

        RuleFor(x => x.Operator)
            .IsInEnum()
            .WithMessage("Invalid operator.");

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

        // Note: Items validation is handled in derived classes since BaseRuleConditionDto doesn't have Items property
    }
}
