using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Helpers;
using ToggleHub.Application.Validators.Flag.Base;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators.Flag.Update;

public class UpdateRuleConditionValidator : RuleConditionValidatorBase<UpdateRuleConditionDto>
{
    public UpdateRuleConditionValidator() : base()
    {
        // ID can be null for new items, but if provided must be > 0
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage("Rule condition ID must be greater than 0 if provided.");

        // Validate items when field type is List
        When(x => EnumHelpers.ParseEnum<RuleFieldType>(x.FieldType) == RuleFieldType.List, () =>
        {
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Items are required when field type is List.");

            RuleForEach(x => x.Items)
                .SetValidator(new UpdateRuleConditionItemValidator())
                .WithMessage("Rule condition item validation failed.");
        });
    }
}
