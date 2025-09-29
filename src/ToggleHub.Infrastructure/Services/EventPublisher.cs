using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToggleHub.Application.EventHandlers;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Events;

namespace ToggleHub.Infrastructure.Services;

public class EventPublisher : IEventPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IServiceProvider serviceProvider, ILogger<EventPublisher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        var handlers = _serviceProvider.GetServices<IConsumer<TEvent>>();
        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleEventAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling event {EventType} in handler {HandlerType}", typeof(TEvent).Name, handler.GetType().Name);
            }
        }
    }
}