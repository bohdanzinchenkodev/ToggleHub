using Microsoft.AspNetCore.Mvc;
using ToggleHub.API.Filters;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Constants;

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

    [HttpGet("{organizationId:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ViewOrganization)]
    public async Task<IActionResult> GetById(int organizationId)
    {
        var result = await _organizationService.GetByIdAsync(organizationId);
        if (result == null)
            return NotFound("Organization not found");
        
        return Ok(result);
    }

    [HttpGet("{organizationSlug}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ViewOrganization, slugRouteKey: nameof(organizationSlug))]
    public async Task<IActionResult> GetBySlug(string organizationSlug)
    {
        var result = await _organizationService.GetBySlugAsync(organizationSlug);
        if (result == null)
            return NotFound("Organization not found");
        
        return Ok(result);
    }
    [HttpPut("{organizationId:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.EditOrganization)]
    public async Task<IActionResult> Update(int organizationId, UpdateOrganizationDto organizationDto)
    {
        organizationDto.Id = organizationId; // Ensure the ID is set for the update
        await _organizationService.UpdateAsync(organizationDto);
        return NoContent();
    }
    
    [HttpDelete("{organizationId:int}")]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.DeleteOrganization)]
    public async Task<IActionResult> Delete(int organizationId)
    {
        await _organizationService.DeleteAsync(organizationId);
        return NoContent();
    }
}