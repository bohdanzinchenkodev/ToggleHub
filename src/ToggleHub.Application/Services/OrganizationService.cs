using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly ISlugGenerator _slugGenerator;
    private readonly IValidator<CreateOrganizationDto> _createValidator;
    private readonly IValidator<UpdateOrganizationDto> _updateValidator;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrgMemberRepository _orgMemberRepository;
    private readonly IWorkContext _workContext;

    public OrganizationService(IOrganizationRepository organizationRepository, IValidator<CreateOrganizationDto> createValidator, ISlugGenerator slugGenerator, IValidator<UpdateOrganizationDto> updateValidator, IOrganizationPermissionService organizationPermissionService, IWorkContext workContext, IOrgMemberRepository orgMemberRepository) 
    {
        _organizationRepository = organizationRepository;
        _createValidator = createValidator;
        _slugGenerator = slugGenerator;
        _updateValidator = updateValidator;
        _workContext = workContext;
        _orgMemberRepository = orgMemberRepository;
    }

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (await _organizationRepository.NameExistsAsync(createDto.Name))
            throw new ApplicationException($"Organization with name {createDto.Name} already exists");

        var entity = createDto.ToEntity();

        entity.Slug = await _slugGenerator.GenerateAsync<Organization>(entity.Name);
        entity.CreatedAt = DateTime.UtcNow;
        entity = await _organizationRepository.CreateAsync(entity);
        
        // Assign the current user as an owner of the new organization
        var userId = _workContext.GetCurrentUserId() ?? throw new UnauthorizedAccessException();
        var orgMember = new OrgMember
        {
            OrganizationId = entity.Id,
            UserId = userId,
            Role = OrgMemberRole.Owner
        };
        await _orgMemberRepository.AddOrgMemberAsync(orgMember);
        
        var dto = entity.ToDto();
        return dto;
    }

    public async Task UpdateAsync(UpdateOrganizationDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var organization = await _organizationRepository.GetByIdAsync(updateDto.Id);
        if(organization == null)
            throw new ApplicationException($"Organization with ID {updateDto.Id} not found");
        
        var slug = organization.Slug;
        // Check if the name has changed to generate a new slug
        if (updateDto.Name != organization.Name)
        {
            if (await _organizationRepository.NameExistsAsync(updateDto.Name))
                throw new ApplicationException($"Organization with name {updateDto.Name} already exists");
            
            slug = await _slugGenerator.GenerateAsync<Organization>(updateDto.Name);
        }
        
        
        organization = updateDto.ToEntity(organization);
        organization.Slug = slug;
        await _organizationRepository.UpdateAsync(organization);
    }

    public async Task<OrganizationDto?> GetByIdAsync(int id)
    {
        var organization = await _organizationRepository.GetByIdAsync(id);
        var dto = organization?.ToDto();
        return dto;
    }
    public async Task<OrganizationDto?> GetBySlugAsync(string slug)
    {
        var organization = await _organizationRepository.GetBySlugAsync(slug);
        var dto = organization?.ToDto();
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var organization = await _organizationRepository.GetByIdAsync(id);
        if (organization == null)    
            throw new ApplicationException($"Organization with ID {id} not found");
    
        await _organizationRepository.DeleteAsync(id);
    }

    public async Task<PagedListDto<OrganizationDto>> GetOrganizationsByUserIdAsync(int userId, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        var list = await _organizationRepository
            .GetOrganizationsByUserIdAsync(userId, pageIndex, pageSize);
        var data = list.Select(e => e.ToDto());
        return new PagedListDto<OrganizationDto>(data, list.TotalCount, list.PageIndex, list.PageSize);
    }

    public async Task<PagedListDto<OrganizationDto>> GetOrganizationsForCurrentUserAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var userId = _workContext.GetCurrentUserId() ?? throw new UnauthorizedAccessException();
        return await GetOrganizationsByUserIdAsync(userId, pageIndex, pageSize);
    }
}
