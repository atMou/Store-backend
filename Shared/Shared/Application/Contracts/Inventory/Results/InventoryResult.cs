using Shared.Application.Contracts.Product.Results;

namespace Shared.Application.Contracts.Inventory.Results;

public record InventoryResult
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ImageUrl { get; init; }
    public string Brand { get; init; }
    public string Slug { get; init; }
    public int TotalStock { get; init; }
    public int TotalReserved { get; init; }
    public int TotalAvailableStock { get; init; }


    public List<ColorVariantResult> ColorVariants { get; init; }
}

public record SizeVariantResult
{
    public Guid Id { get; init; }
    public SizeResult Size { get; init; }
    public StockResult Stock { get; init; }
    public int Reserved { get; init; }
    public int AvailableStock { get; init; }
    public IEnumerable<WarehouseResult> Warehouses { get; init; }

}

public record ColorVariantResult
{
    public Guid Id { get; init; }
    public ColorResult Color { get; init; }
    public IEnumerable<SizeVariantResult> SizeVariants { get; init; }

}


public record WarehouseResult
{
    public string Code { get; init; }
    public string Name { get; init; }
    public string Address { get; init; }
    public string City { get; init; }
    public string? State { get; init; }
    public string Country { get; init; }
    public string? PostalCode { get; init; }
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
}
