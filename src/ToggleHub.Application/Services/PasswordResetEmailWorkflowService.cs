using ToggleHub.Application.Constants;
using ToggleHub.Application.DTOs.Identity;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Application.Services;

public class PasswordResetEmailWorkflowService : IPasswordResetEmailWorkflowService
{
    private readonly IEmailTemplateRenderer _emailTemplateRenderer;
    private readonly IEmailSender _emailSender;

    public PasswordResetEmailWorkflowService(IEmailTemplateRenderer emailTemplateRenderer, IEmailSender emailSender)
    {
        _emailTemplateRenderer = emailTemplateRenderer;
        _emailSender = emailSender;
    }

    public async Task SendPasswordResetEmailAsync(UserDto user, string resetToken)
    {
        // TODO: Use settings to get base URL
        // TODO: Add class to build URL
        var model = new PasswordResetEmailDto
        {
            ResetLink = $"http://localhost:5173/auth/reset-password?token={resetToken}&email={Uri.EscapeDataString(user.Email)}",
            UserName = $"{user.FirstName} {user.LastName}".Trim(),
            ExpiresAt = DateTime.UtcNow.AddHours(1) // Token expires in 1 hour
        };

        var emailBody = await _emailTemplateRenderer.RenderTemplateAsync(EmailTemplates.PasswordReset, model);
        await _emailSender.SendEmailAsync(user.Email, EmailSubjects.PasswordReset, emailBody);
    }
}
