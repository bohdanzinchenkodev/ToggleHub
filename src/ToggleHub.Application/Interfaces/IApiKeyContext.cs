namespace ToggleHub.Application.Interfaces;

public interface IApiKeyContext
{
    int? GetCurrentOrgId();

    int? GetCurrentProjectId();

    int? GetCurrentEnvironmentId();
}