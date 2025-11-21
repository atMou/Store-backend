using Shared.Domain.Errors;

namespace Order.Domain.Models;
public record OrderItem : Entity<OrderItemId>
{
    private OrderItem(ProductId productId, string slug, string imageUrl, int quantity, Money unitPrice) : base(OrderItemId.New)
    {
        ProductId = productId;
        Slug = slug;
        ImageUrl = imageUrl;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    public ProductId ProductId { get; init; }
    public string Slug { get; init; }
    public string Sku { get; init; }
    public string ImageUrl { get; init; }
    public int Quantity { get; init; }
    public Money UnitPrice { get; init; }
    //public Attributes Attributes { get; init; } = new();
    public Money LineTotal => Money.FromDecimal(UnitPrice.Value * Quantity);

    public static Fin<OrderItem> Create(ProductId productId, string slug, string sku, string imageUrl, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            return FinFail<OrderItem>(InvalidOperationError.New("Quantity must be greater than zero."));

        return new OrderItem(productId, slug, imageUrl, quantity, unitPrice);
    }
}

