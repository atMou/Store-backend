namespace Product.Application.Events;

public record ProductCreatedEvent(Guid Id) : IDomainEvent
{
}
