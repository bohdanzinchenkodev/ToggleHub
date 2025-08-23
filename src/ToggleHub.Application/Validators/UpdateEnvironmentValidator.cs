using FluentValidation;
using ToggleHub.Application.DTOs.Environment;

namespace ToggleHub.Application.Validators;

public class UpdateEnvironmentValidator : AbstractValidator<UpdateEnvironmentDto>
{
    public UpdateEnvironmentValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
        RuleFor(x => x.Type)
            .IsInEnum();
    }
}
