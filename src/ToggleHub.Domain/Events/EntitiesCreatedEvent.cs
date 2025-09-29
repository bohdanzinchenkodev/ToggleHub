using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Events;

public class EntitiesCreatedEvent<T>(IEnumerable<T> entities) : BaseEvent
    where T : BaseEntity
{
    public IEnumerable<T> Entities { get; } = entities;
}