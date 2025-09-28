using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Helpers;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Identity.Data;
using ToggleHub.Infrastructure.Identity.Entities;
using ToggleHub.Infrastructure.Identity.Mapping;

namespace ToggleHub.Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly ToggleHubIdentityDbContext _toggleHubIdentityDbContext;
    private readonly IWorkContext _workContext;
    private readonly IValidator<UpdateCurrentUserDto> _updateUserValidator;
    private readonly IOrgMemberRepository _orgMemberRepository;
    private readonly ICacheManager _cacheManager;
    private readonly ICacheKeyFactory _cacheKeyFactory;

    public UserService(
        ToggleHubIdentityDbContext toggleHubIdentityDbContext, 
        IWorkContext workContext,
        IValidator<UpdateCurrentUserDto> updateUserValidator,
        IOrgMemberRepository orgMemberRepository, ICacheKeyFactory cacheKeyFactory, ICacheManager cacheManager)
    {
        _toggleHubIdentityDbContext = toggleHubIdentityDbContext;
        _workContext = workContext;
        _updateUserValidator = updateUserValidator;
        _orgMemberRepository = orgMemberRepository;
        _cacheKeyFactory = cacheKeyFactory;
        _cacheManager = cacheManager;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var cacheKey = _cacheKeyFactory.For(nameof(AppUser), new Dictionary<string, object?>
        {
            { nameof(id), id }
        });
        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var user = await _toggleHubIdentityDbContext
                .Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user?.ToUserDto();
        });
    }

    public async Task<IEnumerable<UserDto>> GetUsersByIdsAsync(IEnumerable<int> ids)
    {
        var users = await _toggleHubIdentityDbContext
            .Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
        
        return users.ToUserDtos();
    }
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var cacheKey = _cacheKeyFactory.For(nameof(AppUser), new Dictionary<string, object?>
        {
            { nameof(email), email.ToLower() }
        });
        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var user = await _toggleHubIdentityDbContext
                .Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());
        
            return user?.ToUserDto();
        });
        
    }
    
    public async Task<UserDto> UpdateCurrentUserAsync(UpdateCurrentUserDto updateDto)
    {
        // Validate the input using the validator
        var validationResult = await _updateUserValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var currentUserId = _workContext.GetCurrentUserId() ?? throw new UnauthorizedAccessException();
        
        var user = await _toggleHubIdentityDbContext
            .Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);

        if (user == null)
            throw new ApplicationException("Current user not found");

        // Check if email is being changed and if it already exists
        if (!string.Equals(user.Email, updateDto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await _toggleHubIdentityDbContext.Users
                .FirstOrDefaultAsync(u => u.Email!.ToLower() == updateDto.Email.ToLower() && u.Id != currentUserId);
            
            if (existingUser != null)
                throw new ApplicationException("Email address is already in use by another user");
        }

        user.Email = updateDto.Email;
        user.FirstName = updateDto.FirstName;
        user.LastName = updateDto.LastName;

        await _toggleHubIdentityDbContext.SaveChangesAsync();
        
        // Invalidate cache
        var cacheKeyById = _cacheKeyFactory.For(nameof(AppUser), new Dictionary<string, object?>
        {
            { nameof(user.Id), user.Id }
        });
        await _cacheManager.RemoveAsync(cacheKeyById.Key);

        return user.ToUserDto();
    }

    public async Task<string[]> GetCurrentUserPermissionsAsync(int organizationId)
    {
        var currentUserId = _workContext.GetCurrentUserId() ?? throw new UnauthorizedAccessException();

        var orgMember = await _orgMemberRepository.GetOrgMemberAsync(organizationId, currentUserId);
        if (orgMember == null)
            throw new ApplicationException($"User is not a member of organization with ID {organizationId}");

        return PermissionRoleHelper.GetPermissionsForRole(orgMember.Role);
    }
}