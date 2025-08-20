using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/projects/{projectId}/environments{environmentId}/flags")]
public class FlagController : ControllerBase
{
    private readonly IFlagService _flagService;

    public FlagController(IFlagService flagService)
    {
        _flagService = flagService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int projectId, int environmentId, [FromBody] CreateFlagDto createDto)
    {
        createDto.ProjectId = projectId;
        createDto.EnvironmentId = environmentId;

        var result = await _flagService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { projectId, environmentId, id = result.Id }, result);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(int projectId, int environmentId, [FromBody] UpdateFlagDto updateDto)
    {
        updateDto.ProjectId = projectId;
        updateDto.EnvironmentId = environmentId;

        var result = await _flagService.UpdateAsync(updateDto);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int projectId, int environmentId, int id)
    {
        var result = await _flagService.GetByIdAsync(id);
        if (result == null)
            return NotFound("Flag not found");
        
        return Ok(result);
    }
    
}