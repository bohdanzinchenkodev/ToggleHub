using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(Policy = AuthConstants.AuthPolicies.RequireIdentity)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
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
}