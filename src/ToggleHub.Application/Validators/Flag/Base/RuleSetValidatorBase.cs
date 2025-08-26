using FluentValidation;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators.Flag.Base;

public abstract class RuleSetValidatorBase<T> : AbstractValidator<T> where T : BaseRuleSetDto
{
    protected RuleSetValidatorBase()
    {
        RuleFor(x => x.Priority)
            .GreaterThan(0)
            .WithMessage("Priority must be greater than 0.");

        RuleFor(x => x.Percentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Percentage must be between 0 and 100.");

        RuleFor(x => x.ReturnValueRaw)
            .NotEmpty()
            .WithMessage("Return value is required for non-Boolean flags when ruleset matches.");

        RuleFor(x => x.OffReturnValueRaw)
            .NotEmpty()
            .WithMessage("Off return value is required for non-Boolean flags.");
    }
}
