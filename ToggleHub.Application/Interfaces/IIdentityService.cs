using ToggleHub.Application.DTOs.Identity;

namespace ToggleHub.Application.Interfaces;

public interface IIdentityService
{
    Task<RegistrationResult> RegisterAsync(RegistrationDto registrationDto);
    Task<LoginResult> LoginAsync(LoginDto loginDto);
    Task LogoutAsync();
}