using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/flags")]
public class FlagEvaluationController : ControllerBase
{
    private readonly IFlagEvaluationService _flagEvaluationService;

    public FlagEvaluationController(IFlagEvaluationService flagEvaluationService)
    {
        _flagEvaluationService = flagEvaluationService;
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateFlag([FromBody] FlagEvaluationRequest evaluationRequest)
    {
        var result = await _flagEvaluationService.EvaluateAsync(evaluationRequest);
        return Ok(result);
    }
}