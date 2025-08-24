using System.Text.RegularExpressions;
using FluentValidation;
using Mapster;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly ISlugGenerator _slugGenerator;
    private readonly IValidator<CreateOrganizationDto> _createValidator;
    private readonly IValidator<UpdateOrganizationDto> _updateValidator;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserService _userService;

    public OrganizationService(IOrganizationRepository organizationRepository, IValidator<CreateOrganizationDto> createValidator, ISlugGenerator slugGenerator, IValidator<UpdateOrganizationDto> updateValidator, IUserService userService) 
    {
        _organizationRepository = organizationRepository;
        _createValidator = createValidator;
        _slugGenerator = slugGenerator;
        _updateValidator = updateValidator;
        _userService = userService;
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
            throw new ApplicationException($"Organization with ID {updateDto.Id} not found");
        
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
            throw new ApplicationException($"Organization with ID {id} not found");
    
        await _organizationRepository.DeleteAsync(id);
    }

    public async Task AddUserToOrganizationAsync(int organizationId, int userId)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId);
        if (organization == null)
            throw new ApplicationException($"Organization with ID {organizationId} not found");
        
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            throw new ApplicationException($"User with ID {userId} not found");
        
        if(await IsUserInOrganizationAsync(organizationId, userId))
            throw new ApplicationException($"User with ID {userId} is already in organization with ID {organizationId}");

        var orgMember = new OrgMember
        {
            OrgId = organizationId,
            UserId = userId,
            Role = OrgMemberRole.Editor
        };
        await _organizationRepository.AddOrgMemberAsync(orgMember);
    }

    public async Task RemoveUserFromOrganizationAsync(int organizationId, int userId)
    {
        var orgMember = await _organizationRepository.GetOrgMemberAsync(organizationId, userId);
        if (orgMember == null)
            throw new ApplicationException($"User with ID {userId} is not in organization with ID {organizationId}");

        await _organizationRepository.DeleteOrgMember(orgMember);
    }

    public async Task<IEnumerable<OrgMemberDto>> GetMembersInOrganizationAsync(int organizationId)
    {
        var orgMembers = (await _organizationRepository.GetMembersInOrganizationAsync(organizationId))
            .ToArray();
        var userIds = orgMembers
            .Select(om => om.UserId)
            .ToList();
        var users = (await _userService.GetUsersByIdsAsync(userIds))
            .ToDictionary(u => u.Id, u => u);
        
        var orgMemberDtos = new List<OrgMemberDto>();
        foreach (var orgMember in orgMembers)
        {
            if (!users.TryGetValue(orgMember.UserId, out var user)) 
                continue;
            var orgMemberDto = new OrgMemberDto
            {
                User = user,
                OrgId = orgMember.OrgId,
                Role = orgMember.Role
            };
            orgMemberDtos.Add(orgMemberDto);
        }
            
        return orgMemberDtos;
    }

    public async Task<bool> IsUserInOrganizationAsync(int organizationId, int userId)
    {
        return await _organizationRepository.IsUserInOrganizationAsync(organizationId, userId);
    }
}
