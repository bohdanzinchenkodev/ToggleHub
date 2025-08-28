namespace ToggleHub.Domain.Events;

public class BaseEvent
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}