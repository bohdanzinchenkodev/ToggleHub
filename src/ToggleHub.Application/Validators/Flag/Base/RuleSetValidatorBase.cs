using FluentValidation;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.Validators.Flag;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators.Flag.Base;

public abstract class RuleSetValidatorBase<T> : AbstractValidator<T>, IIgnoreValidator where T : BaseRuleSetDto
{
    protected RuleSetValidatorBase(BaseCreateOrUpdateFlagDto parentFlag)
    {
        RuleFor(x => x.Priority)
            .GreaterThan(0)
            .WithMessage("Priority must be greater than 0.");

        RuleFor(x => x.Percentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Percentage must be between 0 and 100.");
     
        RuleFor(x => x.ReturnValueRaw)
            .NotEmpty().WithMessage("Is required.");

        RuleFor(x => x.OffReturnValueRaw)
            .NotEmpty().WithMessage("Is required.");

        RuleFor(x => x.ReturnValueRaw)
            .Must(x => IsValidForType(x, parentFlag.ReturnValueType))
            .WithMessage(BuildErr(parentFlag.ReturnValueType));

        RuleFor(x => x.OffReturnValueRaw)
            .Must(x => IsValidForType(x, parentFlag.ReturnValueType))
            .WithMessage(BuildErr(parentFlag.ReturnValueType));
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

    private static string BuildErr(ReturnValueType? type) =>
        FlagValueTypeHelper.BuildErr(type);
}
