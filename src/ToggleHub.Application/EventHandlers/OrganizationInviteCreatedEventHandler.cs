using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Events;

namespace ToggleHub.Application.EventHandlers;

public class OrganizationInviteCreatedEventHandler : IConsumer<OrganizationInviteCreatedEvent>
{
    private readonly IInvitationEmailWorkflowService _invitationEmailWorkflowService;

    public OrganizationInviteCreatedEventHandler(IInvitationEmailWorkflowService invitationEmailWorkflowService)
    {
        _invitationEmailWorkflowService = invitationEmailWorkflowService;
    }

    public async Task HandleEventAsync(OrganizationInviteCreatedEvent eventMessage)
    {
        await _invitationEmailWorkflowService.SendInvitationEmailAsync(eventMessage.Invite);
    }
}