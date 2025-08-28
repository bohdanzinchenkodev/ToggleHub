using System.Security.Cryptography;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Application.Services;

public class ApiKeyGenerator : IApiKeyGenerator
{
    public Task<string> GenerateKeyAsync()
    {
        int size = 32;
        var bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);

        // Safer for URLs/HTTP headers than plain Base64
        return Task.FromResult("api_" + Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", ""));
    }
}