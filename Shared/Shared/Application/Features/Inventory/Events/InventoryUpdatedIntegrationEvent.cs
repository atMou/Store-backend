using Shared.Domain.Enums;

namespace Shared.Application.Features.Inventory.Events;

public sealed record InventoryUpdatedIntegrationEvent(
    Guid ProductId,
    string Brand,
    string Slug,
    IEnumerable<InventoryColorVariantDto> ColorVariants

) : IntegrationEvent;


public record InventoryColorVariantDto
{
    public Guid ColorVariantId { get; init; }
    public string Color { get; init; }
    public IEnumerable<InventorySizeVariantDto> SizeVariants { get; init; }
}
public record InventorySizeVariantDto
{
    public Guid SizeVariantId { get; init; }
    public string Size { get; init; }
    public int Stock { get; init; }
    public StockLevel Level { get; init; }
}