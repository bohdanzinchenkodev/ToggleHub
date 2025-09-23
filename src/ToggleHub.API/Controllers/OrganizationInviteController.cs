using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.OrganizationInvite;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:int}/invites")]
[Authorize(Policy = AuthConstants.AuthPolicies.RequireIdentity)]
public class OrganizationInviteController : ControllerBase
{
    private readonly IOrganizationInviteService _organizationInviteService;

    public OrganizationInviteController(IOrganizationInviteService organizationInviteService)
    {
        _organizationInviteService = organizationInviteService;
    }

    [HttpPost]
    public async Task<IActionResult> SendInvite(CreateOrganizationInviteDto createDto)
    {
        var result = await _organizationInviteService.CreateInviteAsync(createDto);
        return Ok(result);
    }

    [HttpPost("accept")]
    public async Task<IActionResult> AcceptInvite(AcceptInviteDto acceptDto)
    {
        await _organizationInviteService.AcceptInviteAsync(acceptDto);
        return Ok();
    }

    [HttpPost("decline/{token}")]
    public async Task<IActionResult> DeclineInvite(string token)
    {
        await _organizationInviteService.DeclineInviteAsync(token);
        return Ok();
    }

    [HttpGet("{token}")]
    public async Task<IActionResult> GetByToken(string token)
    {
        var invite = await _organizationInviteService.GetByTokenAsync(token);
        if (invite == null)
        {
            return NotFound("Invite not found");
        }
        return Ok(invite);
    }

    [HttpGet]
    public async Task<IActionResult> GetByOrganization(int organizationId, [FromQuery] PagingQuery pagingQuery)
    {
        var result = await _organizationInviteService.GetByOrganizationIdAsync(organizationId, pagingQuery.Page - 1, pagingQuery.PageSize);
        return Ok(result);
    }

    [HttpPost("revoke/{inviteId:int}")]
    public async Task<IActionResult> RevokeInvite(int inviteId)
    {
        await _organizationInviteService.RevokeInviteAsync(inviteId);
        return Ok();
    }

    [HttpPost("resend/{inviteId:int}")]
    public async Task<IActionResult> ResendInvite(int inviteId)
    {
        await _organizationInviteService.ResendInviteAsync(inviteId);
        return Ok();
    }
}
