namespace Basket.Application.Events;

public record CartItemPriceUpdatedEvent(int CountAffectedCartItems, ProductId productId)
{
}
