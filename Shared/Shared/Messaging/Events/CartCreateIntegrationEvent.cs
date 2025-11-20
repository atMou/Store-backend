using Shared.Messaging.Abstractions;

namespace Shared.Messaging.Events;

public record CartCreatedIntegrationEvent(Guid CartId, Guid UserId) : IntegrationEvent
{

}
