namespace Order.Domain.Models;
public record OrderItem : IEntity<OrderItemId>
{
    private OrderItem(
        ProductId productId,
        ColorVariantId colorVariantId,
        string color,
        string slug,
        string sku,
        string imageUrl,
        int quantity,
        decimal unitPrice,
        decimal lineTotal,
        Guid sizeVariantId,
        string size
        )
    {

        ProductId = productId;
        ColorVariantId = colorVariantId;
        Slug = slug;
        Sku = sku;
        ImageUrl = imageUrl;
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineTotal = lineTotal;
        SizeVariantId = sizeVariantId;
        Size = size;
        Color = color;
        Id = OrderItemId.New;
    }
    public OrderItemId Id { get; }
    public ProductId ProductId { get; private init; }
    public OrderId OrderId { get; private set; }
    public ColorVariantId ColorVariantId { get; private init; }
    public Guid SizeVariantId { get; private init; }
    public string Slug { get; private init; }
    public string Sku { get; private init; }
    public string Size { get; private init; }
    public string Color { get; private init; }
    public string ImageUrl { get; private init; }
    public int Quantity { get; private init; }
    public decimal UnitPrice { get; private init; }
    public decimal LineTotal { get; private init; }


    public static Fin<OrderItem> Create(CreateOrderItemDto dto)
    {
        if (dto.Quantity <= 0)
            return FinFail<OrderItem>(InvalidOperationError.New("Quantity must be greater than zero."));

        return new OrderItem(dto.ProductId, dto.ColorVariantId, dto.Color, dto.Slug, dto.Sku, dto.ImageUrl, dto.Quantity, dto.UnitPrice, dto.LineTotal, dto.SizeVariantId, dto.Size);
    }

}

