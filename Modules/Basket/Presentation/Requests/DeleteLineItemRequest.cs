using Basket.Application.Features.Cart.DeleteCartItem;

namespace Basket.Presentation.Requests;

public record DeleteLineItemRequest
{
    public Guid CartId { get; init; }
    public Guid ColorVariantId { get; init; }

    public DeleteLineItemCommand ToCommand() =>
        new(
            Shared.Domain.ValueObjects.ColorVariantId.From(ColorVariantId),
            Shared.Domain.ValueObjects.CartId.From(CartId)
        );

}