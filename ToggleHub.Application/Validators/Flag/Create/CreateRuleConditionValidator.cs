using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.Validators.Flag.Base;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators.Flag.Create;

public class CreateRuleConditionValidator : RuleConditionValidatorBase<CreateRuleConditionDto>
{
    public CreateRuleConditionValidator() : base()
    {
        When(x => x.Items.Any(), () =>
        {
            RuleForEach(x => x.Items)
                .SetValidator(new CreateRuleConditionItemValidator());
        });

        When(x => x.FieldType == RuleFieldType.List, () =>
        {
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("List items are required for List field type.");
        });
    }
}
