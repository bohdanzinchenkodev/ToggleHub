using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        try
        {
            var result = await _organizationInviteService.CreateInviteAsync(createDto);
            return Ok(result);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("accept")]
    public async Task<IActionResult> AcceptInvite(AcceptInviteDto acceptDto)
    {
        try
        {
            await _organizationInviteService.AcceptInviteAsync(acceptDto);
            return Ok(new { message = "Invite accepted successfully" });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("decline/{token}")]
    public async Task<IActionResult> DeclineInvite(string token)
    {
        try
        {
            await _organizationInviteService.DeclineInviteAsync(token);
            return Ok(new { message = "Invite declined successfully" });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{token}")]
    public async Task<IActionResult> GetByToken(string token)
    {
        var invite = await _organizationInviteService.GetByTokenAsync(token);
        if (invite == null)
        {
            return NotFound(new { message = "Invite not found" });
        }
        return Ok(invite);
    }

    [HttpGet]
    public async Task<IActionResult> GetByOrganization(int organizationId, int pageIndex = 0, int pageSize = 20)
    {
        var result = await _organizationInviteService.GetByOrganizationIdAsync(organizationId, pageIndex, pageSize);
        return Ok(result);
    }

    [HttpPost("revoke/{inviteId:int}")]
    public async Task<IActionResult> RevokeInvite(int inviteId)
    {
        try
        {
            await _organizationInviteService.RevokeInviteAsync(inviteId);
            return Ok(new { message = "Invite revoked successfully" });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("resend/{inviteId:int}")]
    public async Task<IActionResult> ResendInvite(int inviteId)
    {
        try
        {
            await _organizationInviteService.ResendInviteAsync(inviteId);
            return Ok(new { message = "Invite resent successfully" });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
