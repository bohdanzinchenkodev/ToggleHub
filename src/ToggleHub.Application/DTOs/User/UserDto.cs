using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public IList<UserRoleDto> Roles { get; set; } = new List<UserRoleDto>();
    
}

public class UserRoleDto
{
    public int Id { get; set; }
    public string Role { get; set; } = string.Empty;
}
