using Basket.Application.Features.Cart.AddMultipleCartItems;

namespace Basket.Presentation.Requests;

public record AddMultipleLineItemsRequest
{
    public Guid CartId { get; init; }
    public IEnumerable<AddLineItemDto> Items { get; init; } = [];

    public AddMultipleLineItemsCommand ToCommand() =>
        new AddMultipleLineItemsCommand
        {
            CartId = Shared.Domain.ValueObjects.CartId.From(CartId),
            Items = Items.Select(item => new LineItemRequest
            {
                ProductId = ProductId.From(item.ProductId),
                ColorVariantId = ColorVariantId.From(item.ColorVariantId),
                SizeVariantId = item.SizeVariantId,
                Quantity = item.Quantity
            })
        };
}

public record AddLineItemDto
{
    public Guid ProductId { get; init; }
    public Guid ColorVariantId { get; init; }
    public Guid SizeVariantId { get; init; }
    public int Quantity { get; init; }
}

