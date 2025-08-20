using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.Helpers;
using ToggleHub.Application.Validators.Flag.Base;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators.Flag.Create;

public class CreateRuleConditionValidator : RuleConditionValidatorBase<CreateRuleConditionDto>
{
    public CreateRuleConditionValidator() : base()
    {
        // Validate items when field type is List
        When(x => EnumHelpers.ParseEnum<RuleFieldType>(x.FieldType) == RuleFieldType.List, () =>
        {
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Items are required when field type is List.");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateRuleConditionItemValidator())
                .WithMessage("Rule condition item validation failed.");
        });
    }
}
