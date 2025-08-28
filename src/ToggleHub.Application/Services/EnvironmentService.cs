using FluentValidation;
using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly IValidator<CreateEnvironmentDto> _createValidator;
    private readonly IValidator<UpdateEnvironmentDto> _updateValidator;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IEventPublisher _eventPublisher;

    public EnvironmentService(IEnvironmentRepository environmentRepository, IValidator<CreateEnvironmentDto> createValidator, IValidator<UpdateEnvironmentDto> updateValidator, IEventPublisher eventPublisher)
    {
        _environmentRepository = environmentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _eventPublisher = eventPublisher;
    }

    public async Task<EnvironmentDto> CreateAsync(CreateEnvironmentDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (await _environmentRepository.EnvironmentExistsAsync(createDto.Type!.Value, createDto.ProjectId))
            throw new ApplicationException($"Environment of type {createDto.TypeString} already exists for project");
        
        var environment = createDto.ToEntity();
        environment = await _environmentRepository.CreateAsync(environment);
        return environment.ToDto();
    }

    public async Task<EnvironmentDto> UpdateAsync(UpdateEnvironmentDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var environment = await _environmentRepository.GetByIdAsync(updateDto.Id);
        if (environment == null)
            throw new ApplicationException($"Environment with id {updateDto.Id} not found.");
        
        updateDto.ToEntity(environment);
        await _environmentRepository.UpdateAsync(environment);
        return environment.ToDto();
    }

    public async Task<EnvironmentDto?> GetByIdAsync(int id)
    {
        var environment = await _environmentRepository.GetByIdAsync(id);
        return environment?.ToDto();
    }

    public async Task<IEnumerable<EnvironmentDto>> GetAllAsync(int? projectId = null)
    {
        var entities = await _environmentRepository.GetAllAsync(projectId);
        return entities.Select(e => e.ToDto());
    }

    public async Task DeleteAsync(int id)
    {
        var environment = await _environmentRepository.GetByIdAsync(id);
        if (environment == null)
            throw new ApplicationException($"Environment with id {id} not found.");
        
        await _environmentRepository.DeleteAsync(id);
    }
    
    public Task<IEnumerable<EnvironmentTypeDto>> GetEnvironmentTypesAsync()
    {
        var values = Enum.GetValues(typeof(EnvironmentType))
            .Cast<EnvironmentType>()
            .Select(e => new EnvironmentTypeDto
            {
                Name = e.ToString(),
                Value = (int)e
            });
        return Task.FromResult(values);
    }

    public async Task GenerateMissingEnvironmentsForProjectAsync(int projectId)
    {
        var environments = new List<Environment>();
        var existingEnvs = (await _environmentRepository.GetAllAsync(projectId))
            .Select(e => e.Type)
            .ToHashSet();
        foreach (EnvironmentType envType in Enum.GetValues(typeof(EnvironmentType)))
        {
            if (existingEnvs.Contains(envType))
                continue;
            
            environments.Add(new Environment
            {
                Type = envType,
                ProjectId = projectId
            });
        }
        await _environmentRepository.CreateAsync(environments);
        var eventMessage = new EnvironmentsAddedToProjectEvent
        {
            ProjectId = projectId,
            Environments = environments
        };
        await _eventPublisher.PublishAsync(eventMessage);
    }
}
