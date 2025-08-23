using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Create;

public class CreateFlagValidator : FlagValidatorBase<CreateFlagDto>
{
    public CreateFlagValidator() : base()
    {
        RuleFor(x => x.RuleSets)
            .NotEmpty()
            .WithMessage("At least one ruleset is required.");

        RuleForEach(x => x.RuleSets)
            .SetValidator(new CreateRuleSetValidator())
            .WithMessage("Ruleset validation failed.");
    }
}
