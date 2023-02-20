using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers;

internal class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
{
    private readonly ILogger<OrderPaymentSuccessIntegrationEventHandler> _logger;

    public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
    {
        //Send function
        _logger.LogInformation($"Order payment success with OrderId: {@event.OrderId}");

        return Task.CompletedTask;
    }
}