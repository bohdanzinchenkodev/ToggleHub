using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToggleHub.API.Filters;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Constants;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:int}/members")]
[Authorize(Policy = AuthConstants.AuthPolicies.RequireIdentity)]
public class OrgMemberController : ControllerBase
{
    private readonly IOrgMemberService _organizationService;

    public OrgMemberController(IOrgMemberService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet("{userId:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageMembers)]
    public async Task<IActionResult> GetOrgMember(int organizationId, int userId)
    {
        var result = await _organizationService.GetOrgMemberAsync(organizationId, userId);
        if (result == null)
            return NotFound("Organization member not found");

        return Ok(result);
    }

    [HttpGet]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageMembers)]
    public async Task<IActionResult> GetOrgMembers(int organizationId, [FromQuery] PagingQuery pagingQuery)
    {
        var result =
            await _organizationService.GetMembersInOrganizationAsync(organizationId, pagingQuery.Page - 1,
                pagingQuery.PageSize);
        return Ok(result);
    }

    [HttpPost]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageMembers)]
    public async Task<IActionResult> AddOrgMember(int organizationId, [FromBody] AddUserToOrganizationDto dto)
    {
        await _organizationService.AddUserToOrganizationAsync(dto.OrganizationId, dto.UserId);
        return NoContent();
    }
}