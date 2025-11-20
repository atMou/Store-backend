namespace Basket.Application.Events;

public record CartCreatedEvent(CartId CartId, UserId UserId) : IDomainEvent
{
}
