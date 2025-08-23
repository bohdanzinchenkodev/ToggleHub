using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Validators;

public class CreateOrganizationValidator : AbstractValidator<CreateOrganizationDto>
{
    private readonly IOrganizationRepository _organizationRepository;

    public CreateOrganizationValidator(IOrganizationRepository organizationRepository)
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

    private async Task<bool> BeUniqueNameAsync(CreateOrganizationDto organization, string name, CancellationToken cancellationToken)
    {
        return !await _organizationRepository.NameExistsAsync(name);
    }
}
