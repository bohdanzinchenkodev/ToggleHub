using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.Services;

namespace ToggleHub.API.Controllers;

[ApiController]
public class HomeController : ControllerBase
{

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        return Ok();
    }
}