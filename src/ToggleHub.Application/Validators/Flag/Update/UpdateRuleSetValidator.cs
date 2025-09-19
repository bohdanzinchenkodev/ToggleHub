using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Update;

public class UpdateRuleSetValidator : RuleSetValidatorBase<UpdateRuleSetDto>, IIgnoreValidator
{
    public UpdateRuleSetValidator(UpdateFlagDto parentFlag) : base(parentFlag)
    {
        // ID can be null for new items, but if provided must be > 0
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage("Ruleset ID must be greater than 0 if provided.");

        RuleForEach(x => x.Conditions)
            .SetValidator(new UpdateRuleConditionValidator())
            .WithMessage("Rule condition validation failed.");
    }
}
