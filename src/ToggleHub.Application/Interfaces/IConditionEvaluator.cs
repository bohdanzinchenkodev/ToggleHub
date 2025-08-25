using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Interfaces;

public interface IConditionEvaluator
{
    bool Matches(RuleSet ruleSet, FlagEvaluationContext context);
}