using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrgMemberService : IOrgMemberService
{
    private readonly IUserService _userService;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrgMemberRepository _orgMemberRepository;

    public OrgMemberService(IUserService userService, IOrganizationRepository organizationRepository, IOrgMemberRepository orgMemberRepository)
    {
        _userService = userService;
        _organizationRepository = organizationRepository;
        _orgMemberRepository = orgMemberRepository;
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
                Id = orgMember.Id,
                User = user,
                OrgId = orgMember.OrgId,
                OrgRole = orgMember.Role
            };
            orgMemberDtos.Add(orgMemberDto);
        }
            
        return orgMemberDtos;
    }

    public async Task<OrgMemberDto?> GetOrgMemberAsync(int organizationId, int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            throw new ApplicationException($"User with ID {userId} not found");
        
        var orgMember = await _organizationRepository.GetOrgMemberAsync(organizationId, userId);
        if (orgMember == null)
            throw new ApplicationException($"User with ID {userId} is not in organization with ID {organizationId}");
        
        var orgMemberDto = new OrgMemberDto
        {
            Id = orgMember.Id,
            User = user,
            OrgId = orgMember.OrgId,
            OrgRole = orgMember.Role
        };
        return orgMemberDto;
    }

    public async Task<bool> IsUserInOrganizationAsync(int organizationId, int userId)
    {
        return await _organizationRepository.IsUserInOrganizationAsync(organizationId, userId);
    }
}