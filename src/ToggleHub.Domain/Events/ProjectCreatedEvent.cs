using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Events;

public class ProjectCreatedEvent : BaseEvent
{
    public required Project Project { get; set; }
}