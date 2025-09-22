using ToggleHub.Application.DTOs.User;

namespace ToggleHub.Application.Interfaces;

public interface IUserService
{
    public Task<UserDto?> GetUserByIdAsync(int id);
    public Task<IEnumerable<UserDto>> GetUsersByIdsAsync(IEnumerable<int> ids);
    public Task<UserDto?> GetUserByEmailAsync(string email);
}