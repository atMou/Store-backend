namespace Shared.Application.Contracts.Carts.Results;
public record CartResult
{
    public Guid UserId { get; init; }
    public Guid CartId { get; init; }
    public decimal Total { get; init; }
    public decimal Tax { get; init; }
    public decimal TotalSub { get; init; }
    public decimal Discount { get; init; }
    public decimal TotalDiscounted { get; init; }
    public decimal ShipmentCost { get; init; }
    public AddressResult DeliveryAddress { get; init; }

    public IEnumerable<CouponResult> Coupons { get; init; } = [];

    public IEnumerable<LineItemResult> LineItems { get; init; } = [];
}

public record AddressResult
{
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public uint PostalCode { get; init; }
    public uint HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }
}

public record CouponResult
{
    public Guid Id { get; init; }
    public Guid? CartId { get; init; }
    public Guid? UserId { get; init; }
    public string Code { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal DiscountValue { get; init; }
    public string DiscountType { get; init; } = null!;
    public decimal MinimumPurchaseAmount { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Status { get; init; } = null!;
}

public record LineItemResult
{
    public Guid ProductId { get; init; }
    public Guid ColorVariantId { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }

    public Guid SizeVariantId { get; init; }
    public string Sku { get; init; }
    public string Slug { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}