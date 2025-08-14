using System.Text.RegularExpressions;
using FluentValidation;
using Mapster;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrganizationService
{
    private readonly ISlugGenerator _slugGenerator;
    private readonly IValidator<CreateOrganizationDto> _createValidator;
    private readonly IValidator<UpdateOrganizationDto> _updateValidator;
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationService(IOrganizationRepository organizationRepository, IValidator<CreateOrganizationDto> createValidator, ISlugGenerator slugGenerator, IValidator<UpdateOrganizationDto> updateValidator) 
    {
        _organizationRepository = organizationRepository;
        _createValidator = createValidator;
        _slugGenerator = slugGenerator;
        _updateValidator = updateValidator;
    }

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var entity = createDto.Adapt<Organization>();

        entity.Slug = await _slugGenerator.GenerateAsync<Organization>(entity.Name);
        entity.CreatedAt = DateTime.UtcNow;
        entity = await _organizationRepository.CreateAsync(entity);
        var dto = entity.Adapt<OrganizationDto>();
        return dto;
    }

    public async Task UpdateAsync(UpdateOrganizationDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var organization = await _organizationRepository.GetByIdAsync(updateDto.Id);
        if(organization == null)
            throw new NotFoundException($"Organization with ID {updateDto.Id} not found");
        
        var slug = organization!.Slug;
        // Check if the name has changed to generate a new slug
        if (updateDto.Name != organization.Name)
            slug = await _slugGenerator.GenerateAsync<Organization>(updateDto.Name);
        
        organization = updateDto.Adapt(organization);
        organization.Slug = slug;
        await _organizationRepository.UpdateAsync(organization);
    }

    public async Task<OrganizationDto?> GetByIdAsync(int id)
    {
        var organization = await _organizationRepository.GetByIdAsync(id);
        var dto = organization?.Adapt<OrganizationDto>();
        return dto;
    }
    public async Task<OrganizationDto?> GetBySlugAsync(string slug)
    {
        var organization = await _slugGenerator.GetBySlugAsync<Organization>(slug);
        var dto = organization?.Adapt<OrganizationDto>();
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var organization = await _organizationRepository.GetByIdAsync(id);
        if (organization == null)    
            throw new NotFoundException($"Organization with ID {id} not found");
    
        await _organizationRepository.DeleteAsync(id);
    }
}
