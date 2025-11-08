using Basket.Domain.Contracts;

using Shared.Domain.Abstractions;

namespace Basket.Domain.Models;

public record CartItem : Entity<CartItemId>
{
    private CartItem(
        CartId cartId,
        string productName,
        string sku,
        ProductId productId,
        string imageUrl,
        int quantity,
        Money unitPrice)
        : base(CartItemId.New())
    {
        CartId = cartId;
        ProductName = productName;
        ProductId = productId;
        Sku = sku;
        ImageUrl = imageUrl;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public CartId CartId { get; private init; }
    public ProductId ProductId { get; private init; }
    public string ProductName { get; private init; } = string.Empty;
    public string Sku { get; private init; } = string.Empty;

    public string Slug { get; private init; } = string.Empty;
    public string ImageUrl { get; private init; }

    public int Quantity { get; set; }
    public Money UnitPrice { get; private init; }
    public Money LineTotal => UnitPrice * Quantity;

    public static CartItem Create(
        CreateCartItemDto dto
    )
    {
        return new CartItem(
            CartId.From(dto.CartId),
            dto.Slug,
            dto.Sku,
            ProductId.From(dto.ProductId),
            dto.ImageUrl,
            dto.Quantity,
            Money.FromDecimal(dto.UnitPrice)
        );
    }
    public CartItem UpdateQuantity(int newQuantity)
    {
        return this with { Quantity = newQuantity };
    }
    public CartItem UpdatePrice(decimal newPrice)
    {
        return this with { UnitPrice = Money.FromDecimal(newPrice) };
    }
}