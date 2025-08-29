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
    private readonly IProjectRepository _projectRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    public FlagEvaluationService(IBucketingService bucketingService, IConditionEvaluator conditionEvaluator, IFlagRepository flagRepository, IValidator<FlagEvaluationRequest> requestValidator, IApiKeyContext apiKeyContext, IProjectRepository projectRepository, IEnvironmentRepository environmentRepository)
    {
        _bucketingService = bucketingService;
        _conditionEvaluator = conditionEvaluator;
        _flagRepository = flagRepository;
        _requestValidator = requestValidator;
        _apiKeyContext = apiKeyContext;
        _projectRepository = projectRepository;
        _environmentRepository = environmentRepository;
    }

    public async Task<FlagEvaluationResult> EvaluateAsync(FlagEvaluationRequest request)
    {
        var validationResult = await _requestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var organizationId = _apiKeyContext.GetCurrentOrgId();
        if (organizationId == null)
            throw new UnauthorizedAccessException("API key is not associated with any organization.");

        var environmentId = _apiKeyContext.GetCurrentEnvironmentId();
        if (environmentId == null)
            throw new UnauthorizedAccessException("API key is not authorized for the specified environment.");
        
        var projectId = _apiKeyContext.GetCurrentProjectId();
        if (projectId == null)
            throw new UnauthorizedAccessException("API key is not authorized for the specified project.");
        
        var flag = await _flagRepository.GetFlagByKeyAsync(request.FlagKey, environmentId.Value, projectId.Value);
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