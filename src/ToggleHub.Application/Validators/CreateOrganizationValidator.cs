using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators;

public class CreateOrganizationValidator : AbstractValidator<CreateOrganizationDto>
{
    public CreateOrganizationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Organization name is required")
            .Length(1, 100)
            .WithMessage("Organization name must be between 1 and 100 characters");
    }
    
}
