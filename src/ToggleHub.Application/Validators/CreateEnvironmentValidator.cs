using FluentValidation;
using ToggleHub.Application.DTOs.Environment;

namespace ToggleHub.Application.Validators;

public class CreateEnvironmentValidator : AbstractValidator<CreateEnvironmentDto>
{
    public CreateEnvironmentValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum();
        RuleFor(x => x.OrganizationId)
            .GreaterThan(0);
    }
}
