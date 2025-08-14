using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/projects/{projectId}/environments")]
public class EnvironmentController : ControllerBase
{
    private readonly IEnvironmentService _environmentService;

    public EnvironmentController(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int projectId)
    {
        var result = await _environmentService.GetAllAsync(projectId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int projectId, int id)
    {
        var result = await _environmentService.GetByIdAsync(id);
        if (result == null)
            return NotFound("Environment not found");
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int projectId, [FromBody] CreateEnvironmentDto dto)
    {
        dto.ProjectId = projectId;
        var result = await _environmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { projectId, id = result.Id }, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(int projectId, [FromBody] UpdateEnvironmentDto dto)
    {
        var result = await _environmentService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int projectId, int id)
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
