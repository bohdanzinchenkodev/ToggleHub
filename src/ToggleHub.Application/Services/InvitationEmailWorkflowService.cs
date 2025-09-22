using ToggleHub.Application.Constants;
using ToggleHub.Application.DTOs.OrganizationInvite;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Services;

public class InvitationEmailWorkflowService : IInvitationEmailWorkflowService
{
    private readonly IEmailTemplateRenderer _emailTemplateRenderer;
    private readonly IEmailSender _emailSender;

    public InvitationEmailWorkflowService(IEmailTemplateRenderer emailTemplateRenderer, IEmailSender emailSender)
    {
        _emailTemplateRenderer = emailTemplateRenderer;
        _emailSender = emailSender;
    }

    public async Task SendInvitationEmailAsync(OrganizationInvite invite)
    {
        //todo:
        //use settings to get base url
        //add class to build url
        var model = new InviteEmailDto
        {
            InviteLink = $"http://localhost:5173/invitations/accept?token={invite.Token}",
            ExpiresAt = invite.ExpiresAt,
            OrganizationName = invite.Organization.Name
        };
        var emailBody = await _emailTemplateRenderer.RenderTemplateAsync(EmailTemplates.Invitation, model);
        await _emailSender.SendEmailAsync(invite.Email, EmailSubjects.Invitation, emailBody);
    }
}