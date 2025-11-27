namespace Basket.Domain.Events;

public record CartItemPriceUpdatedDomainEvent(int CountAffectedCartItems, ProductId ProductId)
{
}
