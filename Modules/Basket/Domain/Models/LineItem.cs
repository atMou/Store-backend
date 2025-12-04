namespace Basket.Domain.Models;

public record LineItem
{
    private LineItem()
    {
    }
    private LineItem(
        CartId cartId,
        ProductId productId,
        Money unitPrice,
        string slug,
        string imageUrl,
        int quantity, VariantId variantId)

    {

        CartId = cartId;
        ProductId = productId;
        Slug = slug;
        ImageUrl = imageUrl;
        Quantity = quantity;
        VariantId = variantId;
        UnitPrice = unitPrice;

    }

    public CartId CartId { get; private init; }
    public ProductId ProductId { get; private init; }
    public VariantId VariantId { get; private init; }
    public string Slug { get; private init; } = string.Empty;
    public string ImageUrl { get; private init; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private init; }
    public Money LineTotal => UnitPrice * Quantity;


    public static LineItem Create(
        ProductId ProductId,
        CartId CartId,
        VariantId variantId,
        string Slug,
        string Sku,
        string ImageUrl,
        int Quantity,
        decimal UnitPrice
    )
    {
        return new LineItem(
            CartId,
            ProductId,
            Money.FromDecimal(UnitPrice),
            Slug,
            ImageUrl,
            Quantity,
            variantId
        );
    }

    public LineItem AddQuantity(int quantity)
    {

        return this with { Quantity = Quantity + quantity };
    }
    public LineItem SubtractQuantity(int quantity)
    {
        return this with { Quantity = Quantity - (quantity < 0 ? 0 : Quantity - quantity) };
    }





}

//context.Entry(existing).CurrentValues.SetValues(updated);