using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Create;

public class CreateFlagValidator : FlagValidatorBase<CreateFlagDto>
{
    public CreateFlagValidator() : base()
    {
        When(x => x.RuleSets.Any(), () =>
        {
            RuleForEach(x => x.RuleSets)
                .SetValidator(x => new CreateRuleSetValidator(x))
                .WithMessage("Ruleset validation failed.");
        });
        
    }
}
