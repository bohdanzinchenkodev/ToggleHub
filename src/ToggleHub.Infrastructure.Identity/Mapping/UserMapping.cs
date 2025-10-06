using ToggleHub.Application.DTOs.User;
using ToggleHub.Infrastructure.Identity.Entities;

namespace ToggleHub.Infrastructure.Identity.Mapping;

public static class UserMapping
{
    public static UserDto ToUserDto(this AppUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.UserRoles?.Select(ur => new UserRoleDto
            {
                Id = ur.Role!.Id,
                Role = ur.Role.Name!
            }).ToList() ?? new List<UserRoleDto>(),
        };
    }
    
    public static IEnumerable<UserDto> ToUserDtos(this IEnumerable<AppUser> users)
    {
        return users.Select(ToUserDto);
    }
}