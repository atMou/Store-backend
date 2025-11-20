using Basket.Application.Features.Cart.AddCartItem;

namespace Basket.Presentation.Requests;

public record AddCartItemRequest
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public Guid CartId { get; init; }

    public AddLineItemCommand ToCommand()
    {
        return new AddLineItemCommand
        {
            ProductId = Shared.Domain.ValueObjects.ProductId.From(ProductId),
            Quantity = Quantity,
            CartId = Shared.Domain.ValueObjects.CartId.From(CartId)
        };
    }
}