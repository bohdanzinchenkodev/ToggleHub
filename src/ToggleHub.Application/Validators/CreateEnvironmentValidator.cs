using FluentValidation;
using ToggleHub.Application.DTOs.Environment;

namespace ToggleHub.Application.Validators;

public class CreateEnvironmentValidator : AbstractValidator<CreateEnvironmentDto>
{
    public CreateEnvironmentValidator()
    {
        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage("Environment type is required and must be valid.");
        RuleFor(x => x.ProjectId)
            .GreaterThan(0);
    }
}
