using ToggleHub.Application.DTOs.ApiKey;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public static class ApiKeyMapping
{
    public static ApiKeyDto ToDto(this ApiKey entity) => new ApiKeyDto
    {
        Key = entity.Key,
        ProjectId = entity.ProjectId,
        EnvironmentId = entity.EnvironmentId,
        ExpiresAt = entity.ExpiresAt,
        IsActive = entity.IsActive,
        OrganizationId = entity.OrganizationId
    };
}