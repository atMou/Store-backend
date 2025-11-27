namespace Basket.Domain.Events;

public record CartCreatedDomainEvent(CartId CartId, UserId UserId) : IDomainEvent
{
}
