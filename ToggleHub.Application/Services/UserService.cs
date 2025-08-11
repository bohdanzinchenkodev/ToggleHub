using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class UserService : BaseService<User>
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository) 
        : base(userRepository)
    {
        _userRepository = userRepository;
    }
}
