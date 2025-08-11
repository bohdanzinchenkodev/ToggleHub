using FluentValidation;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Validators;

public class OrganizationValidator : AbstractValidator<Organization>
{
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationValidator(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Organization name is required")
            .Length(1, 100)
            .WithMessage("Organization name must be between 1 and 100 characters")
            .MustAsync(BeUniqueNameAsync)
            .WithMessage("An organization with this name already exists");
    }

    private async Task<bool> BeUniqueNameAsync(Organization organization, string name, CancellationToken cancellationToken)
    {
        if (organization.Id == 0)
        {
            // Creating new organization
            return !await _organizationRepository.NameExistsAsync(name);
        }
        else
        {
            // Updating existing organization
            return !await _organizationRepository.NameExistsAsync(name, organization.Id);
        }
    }
}
