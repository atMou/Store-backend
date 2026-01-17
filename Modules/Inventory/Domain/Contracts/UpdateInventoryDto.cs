namespace Inventory.Domain.Contracts;


public record UpdateInventoryDto
{
    public InventoryId Id { get; init; }
    public IEnumerable<UpdateInventoryColorDto> ColorVariants { get; init; }
}

public record UpdateInventoryColorDto
{
    public ColorVariantId ColorVariantId { get; set; }
    public IEnumerable<UpdateInventorySizeDto> SizeVariants { get; init; }
}

public record UpdateInventorySizeDto
{
    public Guid? SizeVariantId { get; init; }
    public string Size { get; init; }
    public int Stock { get; init; }
    public int Low { get; init; }
    public int Mid { get; init; }
    public int High { get; init; }
    public IEnumerable<string> Warehouses { get; init; }
}
