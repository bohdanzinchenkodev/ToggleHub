using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Events;

public class FlagUpdatedEvent : BaseEvent
{
    public required Flag Flag { get; set; }
}