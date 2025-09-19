using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Create;

public class CreateRuleSetValidator : RuleSetValidatorBase<CreateRuleSetDto>, IIgnoreValidator
{
    public CreateRuleSetValidator(CreateFlagDto parentFlag) : base(parentFlag)
    {
        When(x => x.Conditions.Any(), () =>
        {
            RuleForEach(x => x.Conditions)
                .SetValidator(new CreateRuleConditionValidator());
        });
    }
}
