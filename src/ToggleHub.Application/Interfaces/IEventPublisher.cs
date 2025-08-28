using ToggleHub.Domain.Events;

namespace ToggleHub.Application.Interfaces;

public partial interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : BaseEvent;
    
}