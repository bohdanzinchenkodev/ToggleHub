using FluentValidation;
using ToggleHub.Application.DTOs.Project;

namespace ToggleHub.Application.Validators;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .Length(1, 100)
            .WithMessage("Project with this name already exists");
        
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Project ID must be greater than 0");
    }
    
}