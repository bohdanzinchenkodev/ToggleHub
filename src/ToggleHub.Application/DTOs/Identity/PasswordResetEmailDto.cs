namespace ToggleHub.Application.DTOs.Identity;

public class PasswordResetEmailDto
{
    public string ResetLink { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
