using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class FlagEvaluationService : IFlagEvaluationService
{
    private readonly IBucketingService _bucketingService;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly IFlagRepository _flagRepository;

    public FlagEvaluationService(IBucketingService bucketingService, IConditionEvaluator conditionEvaluator, IFlagRepository flagRepository)
    {
        _bucketingService = bucketingService;
        _conditionEvaluator = conditionEvaluator;
        _flagRepository = flagRepository;
    }

    public async Task<FlagEvaluationResult> EvaluateAsync(int flagId, FlagEvaluationContext context)
    {
        var flag = await _flagRepository.GetByIdAsync(flagId);
        if (flag == null)
            throw new NotFoundException("Flag not found.");

        var ruleSets = flag.RuleSets
            .OrderBy(rs => rs.Priority)
            .ToArray();

        if (!ruleSets.Any())
            return new FlagEvaluationResult()
            {
                Matched = false,
                ValueType = ReturnValueType.Boolean,
                Value = false,
                Reason = "no-ruleset-defined"
            };
        
        foreach (var ruleSet in ruleSets)
        {
            var matches = _conditionEvaluator.Matches(ruleSet, context);
            if (!matches)
                continue;
            
            var passedPercentage = _bucketingService.PassesPercentage(ruleSet.Percentage, ruleSet.BucketingSeed, flagId.ToString(), context.StickyKey);
            if (!passedPercentage)
            {
                return new FlagEvaluationResult()
                {
                    Matched = false,
                    ValueType = ruleSet.ReturnValueType,
                    Value = ruleSet.OffReturnValueRaw,
                    MatchedRuleSetId = ruleSet.Id,
                    Reason = "percentage-miss"
                };
            }
            
            return new FlagEvaluationResult()
            {
                Matched = true,
                ValueType = ruleSet.ReturnValueType,
                Value = ruleSet.ReturnValueRaw,
                MatchedRuleSetId = ruleSet.Id,
                Reason = "matched"
            };
        }
        
        return new FlagEvaluationResult()
        {
            Matched = false,
            ValueType = ReturnValueType.Boolean,
            Value = false,
            Reason = "no-ruleset-matched"
        };
    }
}