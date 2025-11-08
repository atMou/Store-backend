using Shared.Messaging.Abstractions;

namespace Product.Application.Events;
internal record ProductCreatingFailEvent(Error e) : IntegrationEvent
{
}
