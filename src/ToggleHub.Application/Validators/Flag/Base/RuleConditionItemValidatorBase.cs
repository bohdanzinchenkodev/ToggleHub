using FluentValidation;
using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.Validators.Flag.Base;

public abstract class RuleConditionItemValidatorBase<T> : AbstractValidator<T>, IIgnoreValidator where T : BaseRuleConditionItemDto
{
    protected RuleConditionItemValidatorBase()
    {
        // At least one value must be provided
        RuleFor(x => x)
            .Must(item => !string.IsNullOrEmpty(item.ValueString) || item.ValueNumber.HasValue)
            .WithMessage("At least one value (string or number) must be provided.");

        RuleFor(x => x.ValueString)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.ValueString))
            .WithMessage("String value must not exceed 500 characters.");
    }
}
