using Microsoft.AspNetCore.Mvc;

namespace ToggleHub.API.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        return Ok("Welcome to ToggleHub API!");
    }
}