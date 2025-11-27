using Basket.Application.Features.Cart.UpdateCartItem;

namespace Basket.Presentation.Requests;

public record UpdateLineItemRequest
{
    public Guid CartId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public UpdateLineItemCommand ToCommand()
    {
        return new UpdateLineItemCommand
        {
            CartId = Shared.Domain.ValueObjects.CartId.From(CartId),
            ProductId = Shared.Domain.ValueObjects.ProductId.From(ProductId),
            Quantity = Quantity
        };
    }
}