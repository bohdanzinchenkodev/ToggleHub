using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToggleHub.API.Filters;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Constants;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:int}/projects")]
[Authorize(Policy = AuthConstants.AuthPolicies.RequireIdentity)]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpGet]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ViewProjects)]
    public async Task<IActionResult> GetAll(int organizationId, [FromQuery] PagingQuery pagingQuery)
    {
        var result = await _projectService.GetAllAsync(organizationId, pagingQuery.Page - 1, pagingQuery.PageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ViewProjects)]
    public async Task<IActionResult> GetById(int organizationId, int id)
    {
        var result = await _projectService.GetByIdAsync(id);
        if (result == null)
            return NotFound("Project not found");
        
        return Ok(result);
    }
    [HttpGet("{slug}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ViewProjects)]
    public async Task<IActionResult> GetBySlug(int organizationId, string slug)
    {
        var result = await _projectService.GetBySlugAsync(slug, organizationId);
        if (result == null)
            return NotFound("Project not found");
        
        return Ok(result);
    }
    
    [HttpPost]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> Create(int organizationId, CreateProjectDto projectDto)
    {
        projectDto.OrganizationId = organizationId; // Ensure the organization ID is set
        var result = await _projectService.CreateAsync(projectDto);
        return CreatedAtAction(nameof(GetBySlug), new { organizationId = organizationId, slug = result.Slug }, result);
    }
    
    [HttpPut("{id:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> Update(int organizationId, int id, UpdateProjectDto projectDto)
    {
        projectDto.Id = id; // Ensure the ID is set for the update
        var result = await _projectService.UpdateAsync(projectDto);
        return NoContent();
    }
    [HttpDelete("{id:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> Delete(int organizationId, int id)
    {
        await _projectService.DeleteAsync(id);
        return NoContent();
    }
}