namespace Basket.Application.Events;

public record CartItemUpdateFailEvent(ProductId ProductId, Error Error)
{
}
