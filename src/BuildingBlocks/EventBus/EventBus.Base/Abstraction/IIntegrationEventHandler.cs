using EventBus.Base.Events;

namespace EventBus.Base.Abstraction;

public interface IIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandlerBase where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandlerBase
{
}