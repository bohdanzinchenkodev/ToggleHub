using ToggleHub.Domain.Events;

namespace ToggleHub.Application.EventHandlers;

public partial interface IConsumer<in T> where T : BaseEvent
{
    Task HandleEventAsync(T eventMessage);
}