using System.Data;
using FluentValidation;
using Mapster;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Helpers;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class FlagService : IFlagService
{
    private readonly IValidator<CreateFlagDto> _createValidator;
    private readonly IValidator<UpdateFlagDto> _updateValidator;
    private readonly IFlagRepository _flagRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IProjectRepository _projectRepository;

    public FlagService(IValidator<CreateFlagDto> createValidator, IFlagRepository flagRepository, IEnvironmentRepository environmentRepository, IProjectRepository projectRepository, IValidator<UpdateFlagDto> updateValidator)
    {
        _createValidator = createValidator;
        _flagRepository = flagRepository;
        _environmentRepository = environmentRepository;
        _projectRepository = projectRepository;
        _updateValidator = updateValidator;
    }

    public async Task<FlagDto> CreateAsync(CreateFlagDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var environment = await _environmentRepository.GetByIdAsync(createDto.EnvironmentId);
        if (environment == null)
            throw new ValidationException($"Environment with ID {createDto.EnvironmentId} not found.");
        
        var project = await _projectRepository.GetByIdAsync(createDto.ProjectId);
        if (project == null)
            throw new ValidationException($"Project with ID {createDto.ProjectId} not found.");
        
        if(await _flagRepository.ExistsAsync(createDto.Key, createDto.EnvironmentId, createDto.ProjectId))
            throw new ValidationException($"Flag with key '{createDto.Key}' already exists in the environment.");
        
        var flag = createDto.Adapt<Flag>();
        
        flag.UpdatedAt = DateTimeOffset.UtcNow;
        
        //set bucketing seed to a new GUID
        // Clear ReturnValueRaw fields for boolean rulesets
        foreach (var ruleSet in flag.RuleSets)
        {
            ruleSet.BucketingSeed = Guid.NewGuid();
            
            if (ruleSet.ReturnValueType != ReturnValueType.Boolean)
                continue;
            
            ruleSet.OffReturnValueRaw = null;
            ruleSet.ReturnValueRaw = null;
        }
        
        await _flagRepository.CreateAsync(flag);
        return flag.Adapt<FlagDto>();
    }

    public async Task<FlagDto?> GetByIdAsync(int id)
    {
        var flag = await _flagRepository.GetByIdAsync(id);
        return flag?.Adapt<FlagDto>();
    }

    public async Task<FlagDto> UpdateAsync(UpdateFlagDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var flag = await _flagRepository.GetByIdAsync(updateDto.Id);
        if (flag == null)
            throw new ValidationException($"Flag with ID {updateDto.Id} not found.");

        flag.Description = updateDto.Description;
        flag.Enabled = updateDto.Enabled;
        flag.UpdatedAt = DateTimeOffset.UtcNow;
        flag.Key = flag.Key;
        
        ReconcileRuleSets(flag, updateDto);

        await _flagRepository.UpdateAsync(flag);
        return flag.Adapt<FlagDto>();
    }

    public async Task DeleteAsync(int id)
    {
        var flag = await _flagRepository.GetByIdAsync(id);
        if (flag == null)
            throw new NotFoundException($"Flag with ID {id} not found.");

        await _flagRepository.DeleteAsync(flag.Id);
    }

    private void ReconcileRuleSets(Flag flag, UpdateFlagDto updateDto)
    {
        var existingSets = flag.RuleSets.ToDictionary(x => x.Id);
        var keepSetIds = new HashSet<int>();
        foreach (var ruleSetDto in updateDto.RuleSets)
        {
            RuleSet? ruleSet;
            //create 
            if (!ruleSetDto.Id.HasValue)
            {
                ruleSet = ruleSetDto.Adapt<RuleSet>();
                ruleSet.BucketingSeed = Guid.NewGuid();
                flag.RuleSets.Add(ruleSet);
                continue;
            }

            //update existing
            
            if (!existingSets.TryGetValue(ruleSetDto.Id.Value, out ruleSet))
                throw new ValidationException($"RuleSet with ID {ruleSetDto.Id} not found.");
             
            //keep track of the rule set to keep
            keepSetIds.Add(ruleSetDto.Id.Value);
                
            ruleSet.OffReturnValueRaw = ruleSetDto.OffReturnValueRaw;
            ruleSet.ReturnValueRaw = ruleSetDto.ReturnValueRaw;
            ruleSet.ReturnValueType = EnumHelpers.ParseEnum<ReturnValueType>(ruleSetDto.ReturnValueType);
            ruleSet.Percentage = ruleSetDto.Percentage;
            ruleSet.Priority = ruleSetDto.Priority;
            
            ReconcileConditions(ruleSet, ruleSetDto);
        }
        
        //remove rule sets that are not in the update dto
        foreach (var existingSet in existingSets)
        {
            if (keepSetIds.Contains(existingSet.Key))
                continue;
            
            flag.RuleSets.Remove(existingSet.Value);
        }
        
    }
    
    private void ReconcileConditions(RuleSet ruleSet, UpdateRuleSetDto updateDto)
    {
        var existingConditions = ruleSet.Conditions.ToDictionary(x => x.Id);
        var keepConditionIds = new HashSet<int>();
        
        foreach (var conditionDto in updateDto.Conditions)
        {
            RuleCondition? condition;
            //create new condition
            if (!conditionDto.Id.HasValue)
            {
                condition = conditionDto.Adapt<RuleCondition>();
                ruleSet.Conditions.Add(condition);
                continue;
            }

            //update existing condition
            if (!existingConditions.TryGetValue(conditionDto.Id.Value, out condition))
                throw new ValidationException($"Condition with ID {conditionDto.Id} not found.");
            
            //keep track of the condition to keep
            keepConditionIds.Add(conditionDto.Id.Value);
                
            condition.Field = conditionDto.Field;
            condition.FieldType = EnumHelpers.ParseEnum<RuleFieldType>(conditionDto.FieldType);
            condition.Operator = EnumHelpers.ParseEnum<OperatorType>(conditionDto.Operator);
            condition.ValueString = conditionDto.ValueString;
            condition.ValueNumber = conditionDto.ValueNumber;
            condition.ValueBoolean = conditionDto.ValueBoolean;
            
            ReconcileConditionItems(condition, conditionDto);
        }
        
        //remove conditions that are not in the update dto
        foreach (var existingCondition in existingConditions)
        {
            if (keepConditionIds.Contains(existingCondition.Key))
                continue;
            
            ruleSet.Conditions.Remove(existingCondition.Value);
        }
    }
    
    private void ReconcileConditionItems(RuleCondition condition, UpdateRuleConditionDto updateDto)
    {
        var existingItems = condition.Items.ToDictionary(x => x.Id);
        var keepItemIds = new HashSet<int>();
        
        foreach (var itemDto in updateDto.Items)
        {
            RuleConditionItem? item;
            //create new item
            if (!itemDto.Id.HasValue)
            {
                item = itemDto.Adapt<RuleConditionItem>();
                condition.Items.Add(item);
                continue;
            }

            //update existing item
            if (!existingItems.TryGetValue(itemDto.Id.Value, out item))
                throw new ValidationException($"Condition item with ID {itemDto.Id} not found.");
            
            //keep track of the item to keep
            keepItemIds.Add(itemDto.Id.Value);
                
            item.ValueString = itemDto.ValueString;
            item.ValueNumber = itemDto.ValueNumber;
        }
        
        //remove items that are not in the update dto
        foreach (var existingItem in existingItems)
        {
            if (keepItemIds.Contains(existingItem.Key))
                continue;
            
            condition.Items.Remove(existingItem.Value);
        }
    }
}