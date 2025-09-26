using ToggleHub.Application.Constants;
using ToggleHub.Application.DTOs.Identity;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Application.Services;

public class PasswordResetEmailWorkflowService : IPasswordResetEmailWorkflowService
{
    private readonly IEmailTemplateRenderer _emailTemplateRenderer;
    private readonly IEmailSender _emailSender;
    private readonly IUrlBuilder _urlBuilder;

    public PasswordResetEmailWorkflowService(
        IEmailTemplateRenderer emailTemplateRenderer, 
        IEmailSender emailSender,
        IUrlBuilder urlBuilder)
    {
        _emailTemplateRenderer = emailTemplateRenderer;
        _emailSender = emailSender;
        _urlBuilder = urlBuilder;
    }

    public async Task SendPasswordResetEmailAsync(UserDto user, string resetToken)
    {
        var model = new PasswordResetEmailDto
        {
            ResetLink = _urlBuilder.BuildPasswordResetUrl(resetToken, user.Email),
            UserName = $"{user.FirstName} {user.LastName}".Trim(),
            ExpiresAt = DateTime.UtcNow.AddHours(1) // Token expires in 1 hour
        };

        var emailBody = await _emailTemplateRenderer.RenderTemplateAsync(EmailTemplates.PasswordReset, model);
        await _emailSender.SendEmailAsync(user.Email, EmailSubjects.PasswordReset, emailBody);
    }
}
