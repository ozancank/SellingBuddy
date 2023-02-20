using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers;

internal class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

    public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        //Send function
        _logger.LogInformation($"Order payment failed with OrderId: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");

        return Task.CompletedTask;
    }
}