namespace Shared.Messaging.Abstractions;

public record IntegrationEvent
{
	public Guid EventId => Guid.NewGuid();
	public DateTime OccuredOn => DateTime.UtcNow;

	public string EventType => GetType().Name;
}
