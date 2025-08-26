using FluentValidation;
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
    private readonly IValidator<FlagEvaluationRequest> _requestValidator;
    public FlagEvaluationService(IBucketingService bucketingService, IConditionEvaluator conditionEvaluator, IFlagRepository flagRepository, IValidator<FlagEvaluationRequest> requestValidator)
    {
        _bucketingService = bucketingService;
        _conditionEvaluator = conditionEvaluator;
        _flagRepository = flagRepository;
        _requestValidator = requestValidator;
    }

    public async Task<FlagEvaluationResult> EvaluateAsync(FlagEvaluationRequest request)
    {
        var validationResult = await _requestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var flag = await _flagRepository.GetFlagByKeyAsync(request.FlagKey, request.EnvironmentId, request.ProjectId);
        if (flag == null)
            throw new NotFoundException($"Flag not found.");

        var context = new FlagEvaluationContext(request.UserId, request.ConditionAttributes);
        // If the flag itself is disabled, short-circuit and return the off default.
        if (!flag.Enabled)
        {
            return new FlagEvaluationResult()
            {
                Matched = false,
                ValueType = flag.ReturnValueType,
                Value = flag.DefaultValueOffRaw,
                Reason = "flag-disabled"
            };
        }

        var ruleSets = flag.RuleSets
            .OrderBy(rs => rs.Priority)
            .ToArray();

        if (!ruleSets.Any())
            return new FlagEvaluationResult()
            {
                Matched = flag.Enabled,
                ValueType = flag.ReturnValueType,
                Value = flag.Enabled ? flag.DefaultValueOnRaw : flag.DefaultValueOffRaw,
                Reason = "no-ruleset-defined"
            };
        
        foreach (var ruleSet in ruleSets)
        {
            var matches = _conditionEvaluator.Matches(ruleSet, context);
            if (!matches)
                continue;
            
            var passedPercentage = _bucketingService.PassesPercentage(ruleSet.Percentage, ruleSet.BucketingSeed, flag.Id.ToString(), context.StickyKey);
            if (!passedPercentage)
            {
                return new FlagEvaluationResult()
                {
                    Matched = false,
                    ValueType = flag.ReturnValueType,
                    Value = ruleSet.OffReturnValueRaw,
                    MatchedRuleSetId = ruleSet.Id,
                    Reason = "percentage-miss"
                };
            }
            
            return new FlagEvaluationResult()
            {
                Matched = true,
                ValueType = flag.ReturnValueType,
                Value = ruleSet.ReturnValueRaw,
                MatchedRuleSetId = ruleSet.Id,
                Reason = "matched"
            };
        }
        
        return new FlagEvaluationResult()
        {
            Matched = false,
            ValueType = flag.ReturnValueType,
            Value = flag.DefaultValueOffRaw,
            Reason = "no-ruleset-matched"
        };

    }
}