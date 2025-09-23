using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Events;

public class OrganizationInviteCreatedEvent : BaseEvent
{
    public OrganizationInviteCreatedEvent(OrganizationInvite invite)
    {
        Invite = invite;
    }

    public OrganizationInvite Invite { get; private set; }
}