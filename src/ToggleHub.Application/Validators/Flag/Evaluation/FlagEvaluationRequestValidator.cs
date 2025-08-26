using FluentValidation;
using ToggleHub.Application.DTOs.Flag.Evaluation;

namespace ToggleHub.Application.Validators.Flag.Evaluation;

public class FlagEvaluationRequestValidator : AbstractValidator<FlagEvaluationRequest>
{
    public FlagEvaluationRequestValidator() : base()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .NotNull()
            .WithMessage("UserId is required.");
        
        RuleFor(x => x.FlagKey)
            .NotEmpty()
            .NotNull()
            .WithMessage("FlagKey is required.");
        
        
    }
}