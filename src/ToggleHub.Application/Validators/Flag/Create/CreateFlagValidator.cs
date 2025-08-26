using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Create;

public class CreateFlagValidator : FlagValidatorBase<CreateCreateOrUpdateFlagDto>
{
    public CreateFlagValidator() : base()
    {
        When(x => x.RuleSets.Any(), () =>
        {
            RuleForEach(x => x.RuleSets)
                .SetValidator(new CreateRuleSetValidator())
                .WithMessage("Ruleset validation failed.");
        });
        
    }
}
