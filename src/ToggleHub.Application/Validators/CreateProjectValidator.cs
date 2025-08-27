using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Project;

namespace ToggleHub.Application.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .Length(1, 100)
            .WithMessage("Project name must be between 1 and 100 characters");

        RuleFor(x => x.OrganizationId)
            .GreaterThan(0)
            .WithMessage("Organization ID must be greater than 0");
    }

    
}