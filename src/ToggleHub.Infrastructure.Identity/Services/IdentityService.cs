using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using ToggleHub.Application.DTOs.Identity;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Constants;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Infrastructure.Identity.Entities;

namespace ToggleHub.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IWorkContext _workContext;

    public IdentityService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWorkContext workContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _workContext = workContext;
    }

    public async Task<RegistrationResult> RegisterAsync(RegistrationDto registrationDto)
    {
        var user = new AppUser()
        {
            Email = registrationDto.Email,
            UserName = registrationDto.Email,
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName,
        };
        var result = await _userManager.CreateAsync(user, registrationDto.Password);
        if (!result.Succeeded)
            throw new UserCreationFailedException(result.Errors.Select(x => x.Description));

        // Assign default role
        var roleResult = await _userManager.AddToRoleAsync(user, UserConstants.UserRoles.User);
        if (!roleResult.Succeeded)
        {
            // Rollback user creation if role assignment fails
            await _userManager.DeleteAsync(user);
            throw new UserCreationFailedException(roleResult.Errors.Select(x => x.Description));
        }
        
        await _signInManager.SignInAsync(user, isPersistent: false);
        return new();
    }

    public async Task<LoginResult> LoginAsync(LoginDto loginDto)
    {
        var  signInResult = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, false);
        if (!signInResult.Succeeded)
            throw new AuthenticationException("Invalid login attempt.");
        
        return new();
    }

    public Task LogoutAsync()
    {
        return _signInManager.SignOutAsync();
    }
    
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        var userId = _workContext.GetCurrentUserId();
        if (userId == null)
            return null;
        
        var user = await _userManager.FindByIdAsync(userId.ToString() ?? "");
        if (user == null)
            return null;
        
        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        return userDto;
    }
}