using Shared.Messaging.Abstractions;

namespace Shared.Messaging.Events;

public record TestIntegrationEvent(Guid id) : IntegrationEvent
{
}


