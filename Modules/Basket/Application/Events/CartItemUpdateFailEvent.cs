using Shared.Domain.ValueObjects;

namespace Basket.Application.Events;

public record CartItemUpdateFailEvent(ProductId ProductId, Error error)
{
}
