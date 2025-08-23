using ToggleHub.Application.DTOs.Identity;
using ToggleHub.Application.DTOs.User;

namespace ToggleHub.Application.Interfaces;

public interface IIdentityService
{
    Task<RegistrationResult> RegisterAsync(RegistrationDto registrationDto);
    Task<LoginResult> LoginAsync(LoginDto loginDto);
    Task LogoutAsync();
    Task<UserDto?> GetCurrentUserAsync();
}