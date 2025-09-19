using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Update;

public class UpdateRuleConditionItemValidator : RuleConditionItemValidatorBase<UpdateRuleConditionItemDto>, IIgnoreValidator
{
    public UpdateRuleConditionItemValidator() : base()
    {
        // ID can be null for new items, but if provided must be > 0
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage("Rule condition item ID must be greater than 0 if provided.");
    }
}
