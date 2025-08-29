using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/flags")]
[Authorize(Policy = AuthConstants.AuthPolicies.RequireApiKey)]
public class FlagEvaluationController : ControllerBase
{
    private readonly IFlagEvaluationService _flagEvaluationService;
    private readonly IApiKeyContext _apiKeyContext;

    public FlagEvaluationController(IFlagEvaluationService flagEvaluationService, IApiKeyContext apiKeyContext)
    {
        _flagEvaluationService = flagEvaluationService;
        _apiKeyContext = apiKeyContext;
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateFlag([FromBody] FlagEvaluationRequest evaluationRequest)
    {
        var result = await _flagEvaluationService.EvaluateAsync(evaluationRequest);
        return Ok(result);
    }
}