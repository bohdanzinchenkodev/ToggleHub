using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrgMemberService : IOrgMemberService
{
    private readonly IUserService _userService;
    private readonly IOrgMemberRepository _orgMemberRepository;

    public OrgMemberService(IUserService userService, IOrgMemberRepository orgMemberRepository)
    {
        _userService = userService;
        _orgMemberRepository = orgMemberRepository;
    }

    
    public async Task AddUserToOrganizationAsync(int organizationId, int userId)
    {
        var organization = await _orgMemberRepository.GetByIdAsync(organizationId);
        if (organization == null)
            throw new ApplicationException($"Organization with ID {organizationId} not found");
        
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            throw new ApplicationException($"User with ID {userId} not found");
        
        if(await IsUserInOrganizationAsync(organizationId, userId))
            throw new ApplicationException($"User with ID {userId} is already in organization with ID {organizationId}");

        var orgMember = new OrgMember
        {
            OrganizationId = organizationId,
            UserId = userId,
            Role = OrgMemberRole.FlagManager
        };
        await _orgMemberRepository.CreateAsync(orgMember);
    }

    public async Task RemoveUserFromOrganizationAsync(int organizationId, int userId)
    {
        var orgMember = await _orgMemberRepository.GetOrgMemberAsync(organizationId, userId);
        if (orgMember == null)
            throw new ApplicationException($"User with ID {userId} is not in organization with ID {organizationId}");

        await _orgMemberRepository.DeleteAsync(orgMember.Id);
    }

    public async Task RemoveOrgMemberAsync(int orgMemberId)
    {
        var orgMember = await _orgMemberRepository.GetByIdAsync(orgMemberId);
        if (orgMember == null)
            throw new ApplicationException($"Organization member with ID {orgMemberId} not found");
        
        if (orgMember.Role == OrgMemberRole.Owner)
            throw new ApplicationException("Cannot remove organization owner");

        await _orgMemberRepository.DeleteAsync(orgMemberId);
    }

    public async Task<PagedListDto<OrgMemberDto>> GetMembersInOrganizationAsync(int organizationId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var orgMembers = await _orgMemberRepository.GetMembersInOrganizationAsync(organizationId, pageIndex, pageSize);
        var userIds = orgMembers
            .Select(om => om.UserId)
            .ToList();
        var users = (await _userService.GetUsersByIdsAsync(userIds))
            .ToDictionary(u => u.Id, u => u);
        
        var orgMemberDtos = new List<OrgMemberDto>();
        foreach (var orgMember in orgMembers)
        {
            var orgMemberDto = new OrgMemberDto
            {
                Id = orgMember.Id,
                User = users[orgMember.UserId],
                OrganizationId = orgMember.OrganizationId,
                OrganizationRole = orgMember.Role
            };
            orgMemberDtos.Add(orgMemberDto);
        }
            
        return new PagedListDto<OrgMemberDto>(orgMemberDtos, orgMembers.TotalCount, orgMembers.PageIndex, orgMembers.PageSize);
    }

    public async Task<OrgMemberDto?> GetOrgMemberAsync(int organizationId, int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            throw new ApplicationException($"User with ID {userId} not found");
        
        var orgMember = await _orgMemberRepository.GetOrgMemberAsync(organizationId, userId);
        if (orgMember == null)
            throw new ApplicationException($"User with ID {userId} is not in organization with ID {organizationId}");
        
        var orgMemberDto = new OrgMemberDto
        {
            Id = orgMember.Id,
            User = user,
            OrganizationId = orgMember.OrganizationId,
            OrganizationRole = orgMember.Role
        };
        return orgMemberDto;
    }

    public async Task<bool> IsUserInOrganizationAsync(int organizationId, int userId)
    {
        return await _orgMemberRepository.IsUserInOrganizationAsync(organizationId, userId);
    }

    public async Task ChangeOrgMemberRoleAsync(ChangeOrgMemberRoleDto dto)
    {
        var orgMember = await _orgMemberRepository.GetOrgMemberAsync(dto.OrganizationId, dto.UserId);
        if (orgMember == null)
            throw new ApplicationException($"User with ID {dto.UserId} is not in organization with ID {dto.OrganizationId}");

        if (!dto.NewRole.HasValue)
            throw new ApplicationException("New role is invalid");
        
        if(orgMember.Role == OrgMemberRole.Owner)
            throw new ApplicationException("Cannot change role of organization owner");
        
        if(orgMember.Role == dto.NewRole.Value)
            return;
        
        orgMember.Role = dto.NewRole.Value;
        await _orgMemberRepository.UpdateAsync(orgMember);
    }
}