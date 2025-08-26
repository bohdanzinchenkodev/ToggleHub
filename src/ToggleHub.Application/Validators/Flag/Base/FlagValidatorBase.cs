using FluentValidation;
using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.Validators.Flag.Base;

public abstract class FlagValidatorBase<T> : AbstractValidator<T> where T : BaseCreateOrUpdateFlagDto
{
    protected FlagValidatorBase()
    {
        // Common flag validation rules
        RuleFor(x => x.ProjectId)
            .GreaterThan(0)
            .WithMessage("Project ID must be greater than 0.");

        RuleFor(x => x.EnvironmentId)
            .GreaterThan(0)
            .WithMessage("Environment ID must be greater than 0.");

        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Flag Key is required.")
            .MaximumLength(100)
            .WithMessage("Flag Key must not exceed 100 characters.")
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Flag Key can only contain letters, numbers, underscores, and hyphens.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must not exceed 1000 characters.");
    }
}
