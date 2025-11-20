namespace Shared.Messaging.Events;

public record FailCreateCartIntegrationEvent(Guid UserId, string[] Errors)
{
}