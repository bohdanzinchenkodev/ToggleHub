using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Update;

public class UpdateFlagValidator : FlagValidatorBase<UpdateFlagDto>
{
    public UpdateFlagValidator() : base()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Flag ID is required.");

        RuleFor(x => x.RuleSets)
            .NotEmpty()
            .WithMessage("At least one ruleset is required.");

        RuleForEach(x => x.RuleSets)
            .SetValidator(new UpdateRuleSetValidator())
            .WithMessage("Ruleset validation failed.");
    }
}
