using FluentValidation;
using Mapster;
using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly IValidator<CreateEnvironmentDto> _createValidator;
    private readonly IValidator<UpdateEnvironmentDto> _updateValidator;
    private readonly IEnvironmentRepository _environmentRepository;

    public EnvironmentService(IEnvironmentRepository environmentRepository, IValidator<CreateEnvironmentDto> createValidator, IValidator<UpdateEnvironmentDto> updateValidator)
    {
        _environmentRepository = environmentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<EnvironmentDto> CreateAsync(CreateEnvironmentDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var environment = createDto.Adapt<Environment>();
        environment = await _environmentRepository.CreateAsync(environment);
        return environment.Adapt<EnvironmentDto>();
    }

    public async Task<EnvironmentDto> UpdateAsync(UpdateEnvironmentDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var environment = await _environmentRepository.GetByIdAsync(updateDto.Id);
        if (environment == null)
            throw new ApplicationException($"Environment with id {updateDto.Id} not found.");
        
        environment.Type = updateDto.Type!.Value;
        await _environmentRepository.UpdateAsync(environment);
        return environment.Adapt<EnvironmentDto>();
    }

    public async Task<EnvironmentDto?> GetByIdAsync(int id)
    {
        var environment = await _environmentRepository.GetByIdAsync(id);
        return environment?.Adapt<EnvironmentDto>();
    }

    public async Task<IEnumerable<EnvironmentDto>> GetAllAsync(int? projectId = null)
    {
        var entities = await _environmentRepository.GetAllAsync(projectId);
        return entities.Adapt<IEnumerable<EnvironmentDto>>();
    }

    public async Task DeleteAsync(int id)
    {
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
}
