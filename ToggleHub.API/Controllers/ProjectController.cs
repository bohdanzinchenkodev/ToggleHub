using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations/{orgId}/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int orgId)
    {
        var result = await _projectService.GetAllAsync(orgId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int orgId, int id)
    {
        var result = await _projectService.GetByIdAsync(id);
        if (result == null)
            return NotFound("Project not found");
        
        return Ok(result);
    }
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(int orgId, string slug)
    {
        var result = await _projectService.GetBySlugAsync(slug);
        if (result == null)
            return NotFound("Project not found");
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(int orgId, CreateProjectDto projectDto)
    {
        projectDto.OrgId = orgId; // Ensure the organization ID is set
        var result = await _projectService.CreateAsync(projectDto);
        return CreatedAtAction(nameof(GetBySlug), new { orgId, slug = result.Slug }, result);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int orgId, int id, UpdateProjectDto projectDto)
    {
        projectDto.Id = id; // Ensure the ID is set for the update
        var result = await _projectService.UpdateAsync(projectDto);
        return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int orgId, int id)
    {
        await _projectService.DeleteAsync(id);
        return NoContent();
    }
}