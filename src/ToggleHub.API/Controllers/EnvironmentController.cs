using Microsoft.AspNetCore.Mvc;
using ToggleHub.API.Filters;
using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Constants;
using ToggleHub.Domain.Entities;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:int}/projects/{projectId:int}/environments")]
public class EnvironmentController : ControllerBase
{
    private readonly IEnvironmentService _environmentService;

    public EnvironmentController(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    [HttpGet]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> GetAll(int organizationId, int projectId)
    {
        var result = await _environmentService.GetAllAsync(projectId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> GetById(int organizationId, int projectId, int id)
    {
        var result = await _environmentService.GetByIdAsync(id);
        if (result == null)
            return NotFound("Environment not found");
        return Ok(result);
    }

    [HttpPost]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> Create(int organizationId, int projectId, [FromBody] CreateEnvironmentDto dto)
    {
        dto.ProjectId = projectId;
        var result = await _environmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { projectId, id = result.Id }, result);
    }

    [HttpPut]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> Update(int organizationId, int projectId, [FromBody] UpdateEnvironmentDto dto)
    {
        var result = await _environmentService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> Delete(int organizationId, int projectId, int id)
    {
        await _environmentService.DeleteAsync(id);
        return NoContent();
    }
    [HttpGet]
    [Route("/api/environment-types")]
    public async Task<IActionResult> GetEnvironmentTypes()
    {
        var envTypes = await _environmentService.GetEnvironmentTypesAsync();
        return Ok(envTypes);
    }
}
