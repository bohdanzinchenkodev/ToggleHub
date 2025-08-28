using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Domain.Events;

public class EnvironmentsAddedToProjectEvent : BaseEvent
{
    public required int ProjectId { get; set; }
    public required List<Environment> Environments { get; set; }
}