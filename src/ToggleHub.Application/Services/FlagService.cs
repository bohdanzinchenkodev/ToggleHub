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
    private readonly IValidator<CreateCreateOrUpdateFlagDto> _createValidator;
    private readonly IValidator<UpdateCreateOrUpdateFlagDto> _updateValidator;
    private readonly IFlagRepository _flagRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IProjectRepository _projectRepository;

    public FlagService(IValidator<CreateCreateOrUpdateFlagDto> createValidator, IFlagRepository flagRepository, IEnvironmentRepository environmentRepository, IProjectRepository projectRepository, IValidator<UpdateCreateOrUpdateFlagDto> updateValidator)
    {
        _createValidator = createValidator;
        _flagRepository = flagRepository;
        _environmentRepository = environmentRepository;
        _projectRepository = projectRepository;
        _updateValidator = updateValidator;
    }

    public async Task<FlagDto> CreateAsync(CreateCreateOrUpdateFlagDto createCreateOrUpdateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createCreateOrUpdateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var environment = await _environmentRepository.GetByIdAsync(createCreateOrUpdateDto.EnvironmentId);
        if (environment == null)
            throw new ApplicationException($"Environment with ID {createCreateOrUpdateDto.EnvironmentId} not found.");
        
        var project = await _projectRepository.GetByIdAsync(createCreateOrUpdateDto.ProjectId);
        if (project == null)
            throw new ApplicationException($"Project with ID {createCreateOrUpdateDto.ProjectId} not found.");
        
        if(await _flagRepository.ExistsAsync(createCreateOrUpdateDto.Key, createCreateOrUpdateDto.EnvironmentId, createCreateOrUpdateDto.ProjectId))
            throw new ApplicationException($"Flag with key '{createCreateOrUpdateDto.Key}' already exists in the environment.");
        
        var flag = createCreateOrUpdateDto.Adapt<Flag>();
        
        flag.UpdatedAt = DateTimeOffset.UtcNow;
        flag.ReturnValueType = createCreateOrUpdateDto.ReturnValueType!.Value; //safe to use ! because of validation
        //set bucketing seed to a new GUID
        foreach (var ruleSet in flag.RuleSets)
        {
            ruleSet.BucketingSeed = Guid.NewGuid();
        }
        
        await _flagRepository.CreateAsync(flag);
        return flag.Adapt<FlagDto>();
    }

    public async Task<FlagDto?> GetByIdAsync(int id)
    {
        var flag = await _flagRepository.GetByIdAsync(id);
        return flag?.Adapt<FlagDto>();
    }

    public async Task<FlagDto> UpdateAsync(UpdateCreateOrUpdateFlagDto updateCreateOrUpdateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateCreateOrUpdateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var flag = await _flagRepository.GetByIdAsync(updateCreateOrUpdateDto.Id);
        if (flag == null)
            throw new ApplicationException($"Flag with ID {updateCreateOrUpdateDto.Id} not found.");

        flag.Description = updateCreateOrUpdateDto.Description;
        flag.Enabled = updateCreateOrUpdateDto.Enabled;
        flag.UpdatedAt = DateTimeOffset.UtcNow;
        flag.Key = flag.Key;
        
        ReconcileRuleSets(flag, updateCreateOrUpdateDto);

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

    private void ReconcileRuleSets(Flag flag, UpdateCreateOrUpdateFlagDto updateCreateOrUpdateDto)
    {
        var existingSets = flag.RuleSets.ToDictionary(x => x.Id);
        var keepSetIds = new HashSet<int>();
        foreach (var ruleSetDto in updateCreateOrUpdateDto.RuleSets)
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
                throw new ApplicationException($"RuleSet with ID {ruleSetDto.Id} not found.");
             
            //keep track of the rule set to keep
            keepSetIds.Add(ruleSetDto.Id.Value);
                
            ruleSet.OffReturnValueRaw = ruleSetDto.OffReturnValueRaw;
            ruleSet.ReturnValueRaw = ruleSetDto.ReturnValueRaw;
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
                throw new ApplicationException($"Condition with ID {conditionDto.Id} not found.");
            
            //keep track of the condition to keep
            keepConditionIds.Add(conditionDto.Id.Value);
                
            condition.Field = conditionDto.Field;
            condition.FieldType = conditionDto.FieldType!.Value;
            condition.Operator = conditionDto.Operator!.Value;
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
                throw new ApplicationException($"Condition item with ID {itemDto.Id} not found.");
            
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