using System.Text.Json;
using FluentValidation;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.Validators.Flag;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators.Flag.Base;

public abstract class FlagValidatorBase<T> : AbstractValidator<T> where T : BaseCreateOrUpdateFlagDto
{
    protected FlagValidatorBase()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0)
            .WithMessage("Project ID must be greater than 0.");

        RuleFor(x => x.EnvironmentId)
            .GreaterThan(0)
            .WithMessage("Environment ID must be greater than 0.");

        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Flag Key is required.")
            .MaximumLength(100).WithMessage("Flag Key must not exceed 100 characters.")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Flag Key can only contain letters, numbers, underscores, and hyphens.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.ReturnValueType)
            .NotNull()
            .WithMessage("Is required.");
        // STRING
        When(x => x.ReturnValueType == ReturnValueType.String, () =>
        {
            RuleFor(x => x.DefaultValueOffRaw)
                .NotEmpty().WithMessage("Is required for String type.")
                .MaximumLength(500).WithMessage("Must not exceed 500 characters.");

            RuleFor(x => x.DefaultValueOnRaw)
                .NotEmpty().WithMessage("Is required for String type.")
                .MaximumLength(500).WithMessage("Must not exceed 500 characters.");
        });

        // BOOLEAN
        When(x => x.ReturnValueType == ReturnValueType.Boolean, () =>
        {
            RuleFor(x => x.DefaultValueOffRaw)
                .NotEmpty().WithMessage("Is required for Boolean type.")
                .Must(FlagValueTypeHelper.IsBoolean).WithMessage("Must be 'true' or 'false' for Boolean type.");

            RuleFor(x => x.DefaultValueOnRaw)
                .NotEmpty().WithMessage("DefaultValueOnRaw is required for Boolean type.")
                .Must(FlagValueTypeHelper.IsBoolean).WithMessage("Must be 'true' or 'false' for Boolean type.");
        });

        // NUMBER
        When(x => x.ReturnValueType == ReturnValueType.Number, () =>
        {
            RuleFor(x => x.DefaultValueOffRaw)
                .NotEmpty().WithMessage("Is required for Number type.")
                .Must(FlagValueTypeHelper.IsNumber).WithMessage("Must be a valid number.");

            RuleFor(x => x.DefaultValueOnRaw)
                .NotEmpty().WithMessage("Is required for Number type.")
                .Must(FlagValueTypeHelper.IsNumber).WithMessage("Must be a valid number.");
        });

        // JSON
        When(x => x.ReturnValueType == ReturnValueType.Json, () =>
        {
            RuleFor(x => x.DefaultValueOffRaw)
                .NotEmpty().WithMessage("Is required for Json type.")
                .Must(FlagValueTypeHelper.IsJson).WithMessage("Must be valid JSON.");

            RuleFor(x => x.DefaultValueOnRaw)
                .NotEmpty().WithMessage("Is required for Json type.")
                .Must(FlagValueTypeHelper.IsJson).WithMessage("Must be valid JSON.");
        });
    }
}