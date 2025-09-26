using ToggleHub.Application.DTOs.User;

namespace ToggleHub.Application.Interfaces;

public interface IPasswordResetEmailWorkflowService
{
    Task SendPasswordResetEmailAsync(UserDto user, string resetToken);
}
