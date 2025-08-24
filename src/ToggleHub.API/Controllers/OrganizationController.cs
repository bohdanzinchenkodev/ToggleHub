using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations")]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrganizationDto organizationDto)
    {
        var result = await _organizationService.CreateAsync(organizationDto);
        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _organizationService.GetByIdAsync(id);
        if (result == null)
            return NotFound("Organization not found");
        
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _organizationService.GetBySlugAsync(slug);
        if (result == null)
            return NotFound("Organization not found");
        
        return Ok(result);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateOrganizationDto organizationDto)
    {
        organizationDto.Id = id; // Ensure the ID is set for the update
        await _organizationService.UpdateAsync(organizationDto);
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _organizationService.DeleteAsync(id);
        return NoContent();
    }
}