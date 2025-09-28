
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
    private readonly IApiKeyContext _apiKeyContext;
    private readonly ICacheManager _cacheManager;
    private readonly IFlagEvaluationCacheKeyFactory _flagEvaluationCacheKeyFactory;

    public FlagEvaluationService(IBucketingService bucketingService, IConditionEvaluator conditionEvaluator,
        IFlagRepository flagRepository, IValidator<FlagEvaluationRequest> requestValidator,
        IApiKeyContext apiKeyContext, ICacheManager cacheManager, IFlagEvaluationCacheKeyFactory flagEvaluationCacheKeyFactory)
    {
        _bucketingService = bucketingService;
        _conditionEvaluator = conditionEvaluator;
        _flagRepository = flagRepository;
        _requestValidator = requestValidator;
        _apiKeyContext = apiKeyContext;

        _cacheManager = cacheManager;
        _flagEvaluationCacheKeyFactory = flagEvaluationCacheKeyFactory;
    }

    public async Task<FlagEvaluationResult> EvaluateAsync(FlagEvaluationRequest request)
    {
        var validationResult = await _requestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var organizationId = _apiKeyContext.GetCurrentOrgId() ?? throw new UnauthorizedAccessException();
        var projectId = _apiKeyContext.GetCurrentProjectId() ?? throw new UnauthorizedAccessException();
        var environmentId = _apiKeyContext.GetCurrentEnvironmentId() ?? throw new UnauthorizedAccessException();
        
        var context = new FlagEvaluationContext(request.UserId, request.ConditionAttributes);
        
        var cacheKey = _flagEvaluationCacheKeyFactory.CreateCacheKey(organizationId, projectId, environmentId, request.FlagKey, context);

        var result = await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var flag = await _flagRepository.GetFlagByKeyAsync(request.FlagKey, environmentId, projectId);
            if (flag == null)
                throw new NotFoundException("Flag not found.");

            return EvaluateInternal(flag, context);
        });
        
        return result;
        
    }
    
    private FlagEvaluationResult EvaluateInternal(Flag flag, FlagEvaluationContext context)
    {
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