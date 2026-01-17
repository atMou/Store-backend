using Basket.Application.Features.Cart.AddCartItem;

namespace Basket.Presentation.Requests;

public record AddLineItemRequest
{
    public Guid ProductId { get; init; }
    public Guid ColorVariantId { get; init; }
    public Guid CartId { get; init; }
    public Guid SizeVariantId { get; init; }
    public int Quantity { get; init; }



    public AddLineItemCommand ToCommand()
    {
        return new AddLineItemCommand
        {
            ProductId = Shared.Domain.ValueObjects.ProductId.From(ProductId),
            Quantity = Quantity,
            CartId = Shared.Domain.ValueObjects.CartId.From(CartId),
            ColorVariantId = Shared.Domain.ValueObjects.ColorVariantId.From(ColorVariantId),
            SizeVariantId = SizeVariantId
        };
    }
}