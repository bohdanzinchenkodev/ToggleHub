using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;

namespace ToggleHub.Application.Validators;

public class UpdateOrganizationValidator : AbstractValidator<UpdateOrganizationDto>
{
    public UpdateOrganizationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Organization name is required")
            .Length(1, 100)
            .WithMessage("Organization name must be between 1 and 100 characters");

        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Organization ID must be greater than 0");
    }
    
}