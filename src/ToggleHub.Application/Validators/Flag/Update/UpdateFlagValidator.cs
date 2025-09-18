using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Update;

public class UpdateFlagValidator : FlagValidatorBase<UpdateFlagDto>
{
    public UpdateFlagValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Flag ID is required.");

        When(x => x.RuleSets.Any(), () =>
        {
            RuleForEach(x => x.RuleSets)
                .Custom((ruleSet, context) => {
                    var flag = context.InstanceToValidate;
                    var validator = new UpdateRuleSetValidator();
                    
                    // Create validation context with parent data
                    var ruleSetContext = new ValidationContext<UpdateRuleSetDto>(ruleSet);
                    ruleSetContext.RootContextData["ParentReturnValueType"] = flag.ReturnValueType;
                    
                    var result = validator.Validate(ruleSetContext);
                    if (!result.IsValid)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.AddFailure(error.PropertyName, error.ErrorMessage);
                        }
                    }
                });
        });
    }
}
