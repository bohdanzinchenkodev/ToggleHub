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
    private readonly IUrlBuilder _urlBuilder;

    public InvitationEmailWorkflowService(
        IEmailTemplateRenderer emailTemplateRenderer, 
        IEmailSender emailSender,
        IUrlBuilder urlBuilder)
    {
        _emailTemplateRenderer = emailTemplateRenderer;
        _emailSender = emailSender;
        _urlBuilder = urlBuilder;
    }

    public async Task SendInvitationEmailAsync(OrganizationInvite invite)
    {
        var model = new InviteEmailDto
        {
            InviteLink = _urlBuilder.BuildOrganizationInviteAcceptUrl(invite.OrganizationId, invite.Token),
            DeclineLink = _urlBuilder.BuildOrganizationInviteDeclineUrl(invite.OrganizationId, invite.Token),
            ExpiresAt = invite.ExpiresAt,
            OrganizationName = invite.Organization.Name
        };
        var emailBody = await _emailTemplateRenderer.RenderTemplateAsync(EmailTemplates.Invitation, model);
        await _emailSender.SendEmailAsync(invite.Email, EmailSubjects.Invitation, emailBody);
    }
}