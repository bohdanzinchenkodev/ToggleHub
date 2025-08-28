namespace ToggleHub.Application.Interfaces;

public interface IApiKeyGenerator
{
    Task<string> GenerateKeyAsync();
}