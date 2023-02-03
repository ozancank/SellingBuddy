using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.UnitTest;

[TestClass]
public class EventBusTests
{
    private ServiceCollection _services;

    public EventBusTests()
    {
        _services = new ServiceCollection();
        _services.AddLogging(configure => configure.AddConsole());
    }

    [TestMethod]
    public void Subscribe_event_on_rabbitmq_test()
    {
        _services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetRabbitMQConfig(), sp);
        });

        var sp = _services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        // eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
    }

    [TestMethod]
    public void Subscribe_event_on_azure_test()
    {
        _services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetAzureConfig(), sp);
        });

        var sp = _services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
    }

    [TestMethod]
    public void Send_message_to_rabbitmq_test()
    {
        _services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetRabbitMQConfig(), sp);
        });

        var sp = _services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    [TestMethod]
    public void Send_message_to_azure_test()
    {
        _services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetAzureConfig(), sp);
        });

        var sp = _services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    private static EventBusConfig GetAzureConfig()
    {
        return new EventBusConfig
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = "EventBus.UnitTest",
            DefaultTopicName = "SellingBuddyTopicName",
            EventBusType = EventBusType.AzureServiceBus,
            EventNameSuffix = "IntegrationEvent",
            EventBusConnectionString = "",
        };
    }

    private static EventBusConfig GetRabbitMQConfig()
    {
        return new EventBusConfig
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = "EventBus.UnitTest",
            DefaultTopicName = "SellingBuddyTopicName",
            EventBusType = EventBusType.RabbitMQ,
            EventNameSuffix = "IntegrationEvent",
            Connection = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
            }
        };
    }
}