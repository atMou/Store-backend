using Basket.Application.Features.Cart.DeleteCartItem;

namespace Basket.Presentation.Requests;

public record DeleteLineItemRequest
{
    public Guid CartId { get; init; }
    public Guid VariantId { get; init; }

    public DeleteLineItemCommand ToCommand() =>
        new(
            ColorVariantId.From(VariantId),
            Shared.Domain.ValueObjects.CartId.From(CartId)
        );

}