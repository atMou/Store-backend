using Shared.Messaging.Abstractions;

namespace Shared.Messaging.Events;

public record UserEmailVerifiedIntegrationEvent(Guid UserId) : IntegrationEvent
{
}
