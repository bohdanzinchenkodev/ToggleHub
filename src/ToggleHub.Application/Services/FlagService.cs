using System.Data;
using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Helpers;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;
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
    private readonly IEventPublisher _eventPublisher;

    public FlagService(IValidator<CreateFlagDto> createValidator, IFlagRepository flagRepository, IEnvironmentRepository environmentRepository, IProjectRepository projectRepository, IValidator<UpdateFlagDto> updateValidator, IEventPublisher eventPublisher)
    {
        _createValidator = createValidator;
        _flagRepository = flagRepository;
        _environmentRepository = environmentRepository;
        _projectRepository = projectRepository;
        _updateValidator = updateValidator;
        _eventPublisher = eventPublisher;
    }

    public async Task<FlagDto> CreateAsync(CreateFlagDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var environment = await _environmentRepository.GetByIdAsync(createDto.EnvironmentId);
        if (environment == null)
            throw new ApplicationException($"Environment with ID {createDto.EnvironmentId} not found.");
        
        var project = await _projectRepository.GetByIdAsync(createDto.ProjectId);
        if (project == null)
            throw new ApplicationException($"Project with ID {createDto.ProjectId} not found.");
        
        if(await _flagRepository.ExistsAsync(createDto.Key, createDto.EnvironmentId, createDto.ProjectId))
            throw new ApplicationException($"Flag with key '{createDto.Key}' already exists in the environment.");
        
        var flag = createDto.ToEntity();
        
        flag.UpdatedAt = DateTimeOffset.UtcNow;
        flag.ReturnValueType = createDto.ReturnValueType!.Value; //safe to use ! because of validation
        //set bucketing seed to a new GUID
        foreach (var ruleSet in flag.RuleSets)
        {
            ruleSet.BucketingSeed = Guid.NewGuid();
        }
        
        await _flagRepository.CreateAsync(flag);
        return flag.ToDto();
    }

    public async Task<FlagDto?> GetByIdAsync(int id)
    {
        var flag = await _flagRepository.GetByIdAsync(id);
        return flag?.ToDto();
    }

    public async Task<FlagDto> UpdateAsync(UpdateFlagDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var flag = await _flagRepository.GetByIdAsync(updateDto.Id);
        if (flag == null)
            throw new ApplicationException($"Flag with ID {updateDto.Id} not found.");

        updateDto.UpdateEntity(flag);
        flag.UpdatedAt = DateTimeOffset.UtcNow;
        
        ReconcileRuleSets(flag, updateDto);

        await _flagRepository.UpdateAsync(flag);
        
        var eventMessage = new FlagUpdatedEvent
        {
            Flag = flag
        };
        await _eventPublisher.PublishAsync(eventMessage);
        
        return flag.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var flag = await _flagRepository.GetByIdAsync(id);
        if (flag == null)
            throw new NotFoundException($"Flag with ID {id} not found.");
        
        var eventMessage = new FlagDeletedEvent
        {
            Flag = flag
        };
        await _eventPublisher.PublishAsync(eventMessage);

        await _flagRepository.DeleteAsync(flag.Id);
    }

    public async Task<PagedListDto<FlagDto>> GetAllAsync(int? projectId = null, int? environmentId = null, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {

        var list = await _flagRepository
            .GetAllAsync(projectId, environmentId, pageIndex, pageSize);
        var data = list.Select(e => e.ToDto());
        return new PagedListDto<FlagDto>(data, list.TotalCount, list.PageIndex, list.PageSize);

    }

    public async Task SetEnabledAsync(int id, bool isEnabled)
    {
        var flag = await _flagRepository.GetByIdAsync(id);
        if (flag == null)
            throw new ApplicationException($"Flag with ID {id} not found.");
        
        flag.Enabled = isEnabled;
        flag.UpdatedAt = DateTimeOffset.UtcNow;
        await _flagRepository.UpdateAsync(flag);
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
                ruleSet = ruleSetDto.ToEntity();
                ruleSet.BucketingSeed = Guid.NewGuid();
                flag.RuleSets.Add(ruleSet);
                continue;
            }

            //update existing
            
            if (!existingSets.TryGetValue(ruleSetDto.Id.Value, out ruleSet))
                throw new ApplicationException($"RuleSet with ID {ruleSetDto.Id} not found.");
             
            //keep track of the rule set to keep
            keepSetIds.Add(ruleSetDto.Id.Value);
                
            ruleSetDto.UpdateEntity(ruleSet);
            
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
                condition = conditionDto.ToEntity();
                ruleSet.Conditions.Add(condition);
                continue;
            }

            //update existing condition
            if (!existingConditions.TryGetValue(conditionDto.Id.Value, out condition))
                throw new ApplicationException($"Condition with ID {conditionDto.Id} not found.");
            
            //keep track of the condition to keep
            keepConditionIds.Add(conditionDto.Id.Value);
                
            conditionDto.UpdateEntity(condition);
            
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
                item = itemDto.ToEntity();
                condition.Items.Add(item);
                continue;
            }

            //update existing item
            if (!existingItems.TryGetValue(itemDto.Id.Value, out item))
                throw new ApplicationException($"Condition item with ID {itemDto.Id} not found.");
            
            //keep track of the item to keep
            keepItemIds.Add(itemDto.Id.Value);
                
            itemDto.UpdateEntity(item);
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