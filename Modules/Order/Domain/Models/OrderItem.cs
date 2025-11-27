using Order.Domain.Contracts;

namespace Order.Domain.Models;
public record OrderItem : Entity<OrderItemId>
{
    private OrderItem(
        ProductId productId,
        string slug,
        string sku,
        string imageUrl,
        int quantity,
        decimal unitPrice,
        decimal lineTotal

    ) : base(OrderItemId.New)
    {
        ProductId = productId;
        Slug = slug;
        Sku = sku;
        ImageUrl = imageUrl;
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineTotal = lineTotal;
    }
    public ProductId ProductId { get; init; }
    public string Slug { get; init; }
    public string Sku { get; init; }
    public string ImageUrl { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }

    public static Fin<OrderItem> Create(CreateOrderItemDto dto)
    {
        if (dto.Quantity <= 0)
            return FinFail<OrderItem>(InvalidOperationError.New("Quantity must be greater than zero."));

        return new OrderItem(dto.ProductId, dto.Slug, dto.Sku, dto.ImageUrl, dto.Quantity, dto.UnitPrice, dto.LineTotal);
    }
}

