using System.Text.RegularExpressions;
using FluentValidation;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrganizationService : BaseService<Organization>
{
    private readonly SlugGenerator _slugGenerator;
    private readonly IValidator<Organization> _validator;

    public OrganizationService(IOrganizationRepository organizationRepository, IValidator<Organization> validator, SlugGenerator slugGenerator) 
        : base(organizationRepository)
    {
        _validator = validator;
        _slugGenerator = slugGenerator;
    }

    public override async Task<Organization> CreateAsync(Organization entity)
    {
        // Validate using FluentValidation
        var validationResult = await _validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        entity.Slug = await _slugGenerator.GenerateAsync<Organization>(entity.Name);
        entity.CreatedAt = DateTime.UtcNow;
        return await base.CreateAsync(entity);
    }

    public override async Task UpdateAsync(Organization entity)
    {
        // Validate using FluentValidation
        var validationResult = await _validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await base.UpdateAsync(entity);
    }

   
}
