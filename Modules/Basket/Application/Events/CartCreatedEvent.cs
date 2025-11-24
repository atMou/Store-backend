namespace Basket.Application.Events;

public record CartCreatedDomainEvent(CartId CartId, UserId UserId) : IDomainEvent
{
}
