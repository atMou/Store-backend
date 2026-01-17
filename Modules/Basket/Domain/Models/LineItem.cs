namespace Basket.Domain.Models;

public record LineItem
{
    private LineItem()
    {
    }

    private LineItem(
        CartId cartId,
        ProductId productId,
        ColorVariantId colorVariantId,
        Money unitPrice,
        string slug,
        string sku,
        string color,
        string size,
        string imageUrl,
        int quantity,
        Guid sizeVariantId)
    {
        CartId = cartId;
        ProductId = productId;
        ColorVariantId = colorVariantId;
        Slug = slug;
        Sku = sku;
        Color = color;
        Size = size;
        ImageUrl = imageUrl;
        Quantity = quantity;
        SizeVariantId = sizeVariantId;
        UnitPrice = unitPrice;
    }

    public CartId CartId { get; private init; }
    public ProductId ProductId { get; private init; }
    public ColorVariantId ColorVariantId { get; private init; }
    public Guid SizeVariantId { get; private init; }
    public string Sku { get; private set; }
    public string Slug { get; private init; }
    public string Color { get; private init; }
    public string Size { get; private init; }
    public string ImageUrl { get; private init; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private init; }
    public Money LineTotal => UnitPrice * Quantity;

    public static LineItem Create(
        ProductId productId,
        CartId cartId,
        ColorVariantId colorVariantId,
        Guid sizeVariantId,
        string slug,
        string sku,
        string color,
        string size,
        string imageUrl,
        int quantity,
        decimal unitPrice)
    {
        return new LineItem(
            cartId,
            productId,
            colorVariantId,
            Money.FromDecimal(unitPrice),
            slug,
            sku,
            color,
            size,
            imageUrl,
            quantity,
            sizeVariantId
        );
    }

    public LineItem AddQuantity(int quantity)
    {
        Quantity += quantity;
        return this;
    }

    public LineItem SubtractQuantity(int quantity)
    {
        Quantity -= quantity < 0 ? 0 : quantity;
        return this;
    }

    public LineItem UpdateQuantity(int quantity)
    {
        Quantity = quantity < 0 ? 0 : quantity;
        return this;
    }
}

