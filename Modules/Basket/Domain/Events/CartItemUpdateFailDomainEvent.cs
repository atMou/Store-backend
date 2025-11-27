namespace Basket.Domain.Events;

public record CartItemUpdateFailDomainEvent(ProductId ProductId, Error Error)
{
}
