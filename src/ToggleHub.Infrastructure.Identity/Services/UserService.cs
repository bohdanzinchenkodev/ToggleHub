using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Identity.Data;
using ToggleHub.Infrastructure.Identity.Mapping;

namespace ToggleHub.Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly ToggleHubIdentityDbContext _toggleHubIdentityDbContext;

    public UserService(ToggleHubIdentityDbContext toggleHubIdentityDbContext)
    {
        _toggleHubIdentityDbContext = toggleHubIdentityDbContext;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _toggleHubIdentityDbContext
            .Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return null;

        return user.ToUserDto();
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
    
    
}