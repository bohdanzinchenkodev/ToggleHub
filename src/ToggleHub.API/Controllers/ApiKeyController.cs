using Microsoft.AspNetCore.Mvc;
using ToggleHub.API.Filters;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Constants;

namespace ToggleHub.API.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:int}/projects/{projectId:int}/environments/{environmentId:int}/apikeys")]
public class ApiKeyController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeyController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    [HttpGet]
    [OrgAuthorize(OrganizationConstants.OrganizationPermissions.ManageProjects)]
    public async Task<IActionResult> GetAll (int organizationId, int projectId, int environmentId, [FromQuery] PagingQuery pagingQuery)
    {
        var result = await _apiKeyService.GetApiKeysAsync(organizationId, projectId, environmentId, pagingQuery.Page - 1, pagingQuery.PageSize);
        return Ok(result);
    }
}