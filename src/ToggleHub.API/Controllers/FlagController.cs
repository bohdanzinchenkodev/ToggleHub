using Microsoft.AspNetCore.Mvc;
using ToggleHub.API.Filters;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:int}/projects/{projectId:int}/environments{environmentId:int}/flags")]
public class FlagController : ControllerBase
{
    private readonly IFlagService _flagService;

    public FlagController(IFlagService flagService)
    {
        _flagService = flagService;
    }

    [HttpPost]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> Create(int organizationId, int projectId, int environmentId, [FromBody] CreateCreateOrUpdateFlagDto createCreateOrUpdateDto)
    {
        createCreateOrUpdateDto.ProjectId = projectId;
        createCreateOrUpdateDto.EnvironmentId = environmentId;

        var result = await _flagService.CreateAsync(createCreateOrUpdateDto);
        return CreatedAtAction(nameof(GetById), new { projectId, environmentId, organizationId, id = result.Id }, result);
    }
    
    [HttpPut]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageFlags)]
    public async Task<IActionResult> Update(int organizationId, int projectId, int environmentId, [FromBody] UpdateCreateOrUpdateFlagDto updateCreateOrUpdateDto)
    {
        updateCreateOrUpdateDto.ProjectId = projectId;
        updateCreateOrUpdateDto.EnvironmentId = environmentId;

        var result = await _flagService.UpdateAsync(updateCreateOrUpdateDto);
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
    
}