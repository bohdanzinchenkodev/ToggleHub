using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public CreateProjectValidator(IProjectRepository projectRepository, IOrganizationRepository organizationRepository)
    {
        _projectRepository = projectRepository;
        _organizationRepository = organizationRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .Length(1, 100)
            .WithMessage("Project name must be between 1 and 100 characters")
            .MustAsync(BeUniqueNameAsync)
            .WithMessage("Project with this name already exists");

        RuleFor(x => x.OrgId)
            .GreaterThan(0)
            .WithMessage("Organization ID must be greater than 0")
            .MustAsync(BeExistingOrgAsync);
    }

    private async Task<bool> BeExistingOrgAsync(int orgId, CancellationToken cancellationToken)
    {
        return await _organizationRepository.GetByIdAsync(orgId) != null;
    }

    private async Task<bool> BeUniqueNameAsync(CreateProjectDto project, string name,
        CancellationToken cancellationToken)
    {
        return !await _projectRepository.NameExistsAsync(name);
    }
}