using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;

namespace ToggleHub.Application.EventHandlers;

public class OrganizationInviteCreatedEventHandler : IConsumer<EntityCreatedEvent<OrganizationInvite>>
{
    private readonly IInvitationEmailWorkflowService _invitationEmailWorkflowService;

    public OrganizationInviteCreatedEventHandler(IInvitationEmailWorkflowService invitationEmailWorkflowService)
    {
        _invitationEmailWorkflowService = invitationEmailWorkflowService;
    }

    public async Task HandleEventAsync(EntityCreatedEvent<OrganizationInvite> eventMessage)
    {
        await _invitationEmailWorkflowService.SendInvitationEmailAsync(eventMessage.Entity);
    }
}