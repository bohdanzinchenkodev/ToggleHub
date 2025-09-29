using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Events;

public class EntityDeletedEvent<T>(T entity) : BaseEvent
    where T : BaseEntity
{
    public T Entity {get; } = entity;
}