using Shared.Domain.Contracts.Product;

namespace Order.Domain.Models;
public record OrderItem
{
    public ProductId ProductId { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string Sku { get; init; } = default!;
    public string ImageUrl { get; init; } = default!;
    public int Quantity { get; init; }
    public Money UnitPrice { get; init; }
    public Money LineTotal => Money.FromDecimal(UnitPrice.Value * Quantity);

    public static Fin<OrderItem> Create(ProductDto product, int quantity)
    {
        if (quantity <= 0)
            return FinFail<OrderItem>(Error.New("Quantity must be greater than zero."));

        return new OrderItem
        {
            ProductId = ProductId.From(product.Id),
            Slug = product.Slug,
            Sku = product.Sku,
            ImageUrl = product.ImageUrls[0],
            Quantity = quantity,
            UnitPrice = product.NewPrice
        };
    }
}
