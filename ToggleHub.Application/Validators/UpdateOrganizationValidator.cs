using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Validators;

public class UpdateOrganizationValidator : AbstractValidator<UpdateOrganizationDto>
{
    private readonly IOrganizationRepository _organizationRepository;

    public UpdateOrganizationValidator(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Organization name is required")
            .Length(1, 100)
            .WithMessage("Organization name must be between 1 and 100 characters")
            .MustAsync(BeUniqueNameAsync)
            .WithMessage("An organization with this name already exists");

        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Organization ID must be greater than 0");
    }

    private async Task<bool> BeUniqueNameAsync(UpdateOrganizationDto organization, string name, CancellationToken cancellationToken)
    {
        return !await _organizationRepository.NameExistsAsync(name, organization.Id);
    }
}