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
    [HttpGet("{organizationId:int}/members/{userId:int}")]
    public async Task<IActionResult> GetOrgMember(int organizationId, int userId)
    {
        var result = await _organizationService.GetOrgMemberAsync(organizationId, userId);
        if (result == null)
            return NotFound("Organization member not found");
        
        return Ok(result);
    }
    [HttpPost("{organizationId:int}/members")]
    public async Task<IActionResult> AddOrgMember(int organizationId, [FromBody] AddUserToOrganizationDto dto)
    {
        await _organizationService.AddUserToOrganizationAsync(dto.OrganizationId, dto.UserId);
        return NoContent();
    }
}