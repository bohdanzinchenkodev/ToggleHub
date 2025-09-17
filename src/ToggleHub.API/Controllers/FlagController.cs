using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToggleHub.API.Filters;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Constants;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:int}/projects/{projectId:int}/environments/{environmentId:int}/flags")]
[Authorize(Policy = AuthConstants.AuthPolicies.RequireIdentity)]
public class FlagController : ControllerBase
{
    private readonly IFlagService _flagService;

    public FlagController(IFlagService flagService)
    {
        _flagService = flagService;
    }

    [HttpPost]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> Create(int organizationId, int projectId, int environmentId, [FromBody] CreateFlagDto createDto)
    {
        createDto.ProjectId = projectId;
        createDto.EnvironmentId = environmentId;

        var result = await _flagService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { projectId, environmentId, organizationId, id = result.Id }, result);
    }
    
    [HttpPut]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> Update(int organizationId, int projectId, int environmentId, [FromBody] UpdateFlagDto updateDto)
    {
        updateDto.ProjectId = projectId;
        updateDto.EnvironmentId = environmentId;

        var result = await _flagService.UpdateAsync(updateDto);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> GetById(int organizationId, int projectId, int environmentId, int id)
    {
        var result = await _flagService.GetByIdAsync(id);
        if (result == null)
            return NotFound("Flag not found");
        
        return Ok(result);
    }
    
    [HttpDelete("{id:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> Delete(int projectId, int environmentId, int id)
    {
        await _flagService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> GetAll(int organizationId, int projectId, int environmentId,
        [FromQuery] PagingQuery pagingQuery)
    {
        if(projectId <= 0 || environmentId <= 0)
            return BadRequest("ProjectId and EnvironmentId are required");
        
        var result = await _flagService.GetAllAsync(
            projectId,
            environmentId,
            pagingQuery.Page - 1, 
            pagingQuery.PageSize);
        return Ok(result);
    }
    [HttpPatch("{id:int}/enable")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> Enable(int organizationId, int projectId, int environmentId, int id)
    {
        await _flagService.SetEnabledAsync(id, true);
        return NoContent();
    }

    [HttpPatch("{id:int}/disable")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> Disable(int organizationId, int projectId, int environmentId, int id)
    {
        await _flagService.SetEnabledAsync(id, false);
        return NoContent();
    }

}