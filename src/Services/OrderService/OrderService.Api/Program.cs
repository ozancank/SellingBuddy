using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using OrderService.Api.Extensions;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.Extensions.Registration.ServiceDiscovery;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateOnBuild = false;
}).ConfigureAppConfiguration(i => i.AddConfiguration(builder.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(configure => configure.AddConsole())
                .AddApplicationRegistration(typeof(Program))
                .AddPersistenceRegistration(builder.Configuration)
                .ConfigureEventHandlers()
                .ConfigureConsul(builder.Configuration);

builder.Services.AddSingleton<IEventBus>(sp =>
EventBusFactory.Create(new()
{
    ConnectionRetryCount = 5,
    EventNameSuffix = "IntegrationEvent",
    SubscriberClientAppName = "OrderService",
    EventBusType = EventBusType.RabbitMQ
}, sp));

var app = builder.Build();

app.MigrateDbContext<OrderDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderDbContextSeed>>();
    var dbContextSeeder = new OrderDbContextSeed();
    dbContextSeeder.SeedAsync(context, logger).Wait();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.RegisterWithConsul(app.Lifetime);

var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();