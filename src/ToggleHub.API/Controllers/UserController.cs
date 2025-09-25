using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(Policy = AuthConstants.AuthPolicies.RequireIdentity)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IOrganizationService _organizationService;

    public UserController(IUserService userService, IOrganizationService organizationService)
    {
        _userService = userService;
        _organizationService = organizationService;
    }

    [HttpGet("{id:int}")]
    [NonAction]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound("User not found.");

        return Ok(user);
    }
    
    [HttpGet("me/organizations")]
    public async Task<IActionResult> GetOrganizationsByCurrentUser([FromQuery] PagingQuery pagingQuery)
    {
        var organizations = await _organizationService.GetOrganizationsForCurrentUserAsync(
            pagingQuery.Page - 1, 
            pagingQuery.PageSize);
        return Ok(organizations);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserDto updateDto)
    {
        var updatedUser = await _userService.UpdateCurrentUserAsync(updateDto);
        return Ok(updatedUser);
    }

    [HttpGet("me/permissions/{organizationId:int}")]
    public async Task<IActionResult> GetCurrentUserPermissions(int organizationId)
    {
        var permissions = await _userService.GetCurrentUserPermissionsAsync(organizationId);
        return Ok(permissions);
    }
}