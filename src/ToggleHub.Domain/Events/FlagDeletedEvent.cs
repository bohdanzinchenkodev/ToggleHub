using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Events;

public class FlagDeletedEvent : BaseEvent
{
    public required Flag Flag { get; set; }
}