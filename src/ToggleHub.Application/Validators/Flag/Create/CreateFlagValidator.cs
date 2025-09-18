using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.Validators.Flag.Base;

namespace ToggleHub.Application.Validators.Flag.Create;

public class CreateFlagValidator : FlagValidatorBase<CreateFlagDto>
{
    public CreateFlagValidator()
    {
        When(x => x.RuleSets.Any(), () =>
        {
            RuleForEach(x => x.RuleSets)
                .Custom((ruleSet, context) => {
                    var flag = context.InstanceToValidate;
                    var validator = new CreateRuleSetValidator();
                    
                    // Create validation context with parent data
                    var ruleSetContext = new ValidationContext<CreateRuleSetDto>(ruleSet);
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
