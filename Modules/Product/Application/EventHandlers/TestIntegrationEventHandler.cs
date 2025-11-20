using MassTransit;

using Shared.Messaging.Events;

namespace Product.Application.EventHandlers;

public class TestIntegrationEventHandler : IConsumer<TestIntegrationEvent>
{
    public Task Consume(ConsumeContext<TestIntegrationEvent> context)
    {
        Console.WriteLine($"Test INTEGRATIONEVENT Handled: {context.Message.id}");
        return Task.CompletedTask;
    }
}
