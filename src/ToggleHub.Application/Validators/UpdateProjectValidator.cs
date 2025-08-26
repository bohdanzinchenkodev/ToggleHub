using FluentValidation;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Validators;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
{
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectValidator(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;

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