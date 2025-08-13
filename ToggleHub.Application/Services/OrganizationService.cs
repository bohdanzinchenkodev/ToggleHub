using System.Text.RegularExpressions;
using FluentValidation;
using Mapster;
using ToggleHub.Application.DTOs;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrganizationService
{
    private readonly SlugGenerator _slugGenerator;
    private readonly IValidator<CreateOrganizationDto> _createValidator;
    private readonly IValidator<UpdateOrganizationDto> _updateValidator;
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationService(IOrganizationRepository organizationRepository, IValidator<CreateOrganizationDto> createValidator, SlugGenerator slugGenerator, IValidator<UpdateOrganizationDto> updateValidator) 
    {
        _organizationRepository = organizationRepository;
        _createValidator = createValidator;
        _slugGenerator = slugGenerator;
        _updateValidator = updateValidator;
    }

    public async Task<Organization> CreateAsync(CreateOrganizationDto createDto)
    {
        await _createValidator.ValidateAndThrowAsync(createDto);
        var entity = createDto.Adapt<Organization>();

        entity.Slug = await _slugGenerator.GenerateAsync<Organization>(entity.Name);
        entity.CreatedAt = DateTime.UtcNow;
        return await _organizationRepository.CreateAsync(entity);
    }

    public async Task UpdateAsync(UpdateOrganizationDto updateDto)
    {
        await _updateValidator.ValidateAndThrowAsync(updateDto);
        var entity = await _organizationRepository.GetByIdAsync(updateDto.Id);
        
        var slug = entity!.Slug;
        // Check if the name has changed to generate a new slug
        if (updateDto.Name != entity.Name)
            slug = await _slugGenerator.GenerateAsync<Organization>(updateDto.Name);
        
        entity = updateDto.Adapt(entity);
        entity.Slug = slug;
        await _organizationRepository.UpdateAsync(entity);
    }

   
}
