using Basket.Application.Features.Cart.UpdateCartItem;

namespace Basket.Presentation.Requests;

public record UpdateLineItemRequest
{
    public Guid CartId { get; init; }
    public Guid VariantId { get; init; }
    public int Quantity { get; init; }

    public UpdateLineItemCommand ToCommand()
    {
        return new UpdateLineItemCommand
        {
            CartId = Shared.Domain.ValueObjects.CartId.From(CartId),
            //VariantId = Shared.Domain.ValueObjects.VariantId.From(VariantId),
            Quantity = Quantity,
            VariantId = Shared.Domain.ValueObjects.VariantId.From(VariantId)
        };
    }
}