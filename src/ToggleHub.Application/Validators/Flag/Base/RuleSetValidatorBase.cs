using FluentValidation;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.Validators.Flag;
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
            .NotEmpty().WithMessage("ReturnValueRaw is required.");

        RuleFor(x => x.OffReturnValueRaw)
            .NotEmpty().WithMessage("OffReturnValueRaw is required.");

        // Type-specific validation using root context data
        RuleFor(x => x.ReturnValueRaw)
            .Custom((value, context) => {
                if (context.RootContextData.TryGetValue("ParentReturnValueType", out var typeObj) && 
                    typeObj is ReturnValueType type)
                {
                    if (!IsValidForType(value, type))
                    {
                        context.AddFailure(BuildErr("ReturnValueRaw", type));
                    }
                }
            });

        RuleFor(x => x.OffReturnValueRaw)
            .Custom((value, context) => {
                if (context.RootContextData.TryGetValue("ParentReturnValueType", out var typeObj) && 
                    typeObj is ReturnValueType type)
                {
                    if (!IsValidForType(value, type))
                    {
                        context.AddFailure(BuildErr("OffReturnValueRaw", type));
                    }
                }
            });
    }

    private static bool IsValidForType(string? raw, ReturnValueType? type)
    {
        if (string.IsNullOrWhiteSpace(raw) || type is null) return false;
        return type switch
        {
            ReturnValueType.String => true, // any non-empty string
            ReturnValueType.Boolean => FlagValueTypeHelper.IsBoolean(raw),
            ReturnValueType.Number => FlagValueTypeHelper.IsNumber(raw),
            ReturnValueType.Json => FlagValueTypeHelper.IsJson(raw),
            _ => false
        };
    }

    private static string BuildErr(string field, ReturnValueType? type) =>
        FlagValueTypeHelper.BuildErr(field, type);
}
