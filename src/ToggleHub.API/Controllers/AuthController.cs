using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs.Identity;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationDto registrationDto)
    {
        var result = await _identityService.RegisterAsync(registrationDto);
        return CreatedAtAction(nameof(GetUser), new { id = result.UserId }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _identityService.LoginAsync(loginDto);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _identityService.LogoutAsync();
        return Ok();
    }

    [HttpGet("user/{id}")]
    public IActionResult GetUser(int id)
    {
        return Ok(new { userId = id });
    }
}