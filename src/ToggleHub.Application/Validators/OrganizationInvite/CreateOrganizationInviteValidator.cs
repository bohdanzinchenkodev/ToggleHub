using FluentValidation;
using ToggleHub.Application.DTOs.OrganizationInvite;

namespace ToggleHub.Application.Validators.OrganizationInvite;

public class CreateOrganizationInviteValidator : AbstractValidator<CreateOrganizationInviteDto>
{
    public CreateOrganizationInviteValidator()
    {
        RuleFor(x => x.OrganizationId)
            .GreaterThan(0)
            .WithMessage("Organization ID must be greater than 0.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters.");
    }
}
