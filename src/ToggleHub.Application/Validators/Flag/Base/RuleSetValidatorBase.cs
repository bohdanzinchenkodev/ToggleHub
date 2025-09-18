using FluentValidation;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.Helpers;
using ToggleHub.Application.Validators.Flag;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators.Flag.Base;

public abstract class RuleSetValidatorBase<T> : AbstractValidator<T> where T : BaseRuleSetDto
{
    protected RuleSetValidatorBase(BaseCreateOrUpdateFlagDto parentFlag)
    {
        RuleFor(x => x.Priority)
            .GreaterThan(0)
            .WithMessage("Priority must be greater than 0.");

        RuleFor(x => x.Percentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Percentage must be between 0 and 100.");
     
        // Core presence rule (adjust if one of them can be optional)
        RuleFor(x => x.ReturnValueRaw)
            .NotEmpty().WithMessage("ReturnValueRaw is required.");

        RuleFor(x => x.OffReturnValueRaw)
            .NotEmpty().WithMessage("OffReturnValueRaw is required.");

        // Type-specific validation
        RuleFor(x => x.ReturnValueRaw)
            .Must(value => IsValidForType(value, parentFlag.ReturnValueType))
            .WithMessage(_ => BuildErr("ReturnValueRaw", parentFlag.ReturnValueType));

        RuleFor(x => x.OffReturnValueRaw)
            .Must(value => IsValidForType(value, parentFlag.ReturnValueType))
            .WithMessage(_ => BuildErr("OffReturnValueRaw", parentFlag.ReturnValueType));
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
