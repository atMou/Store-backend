using Shared.Domain.Enums;

namespace Shared.Application.Features.Inventory.Events;

public sealed record InventoryUpdatedIntegrationEvent(
    Guid ProductId,
    IEnumerable<UpdateColorVariantDto> ColorVariants

) : IntegrationEvent;


public record UpdateColorVariantDto
{
    public Guid ColorVariantId { get; init; }
    public IEnumerable<UpdateSizeVariantDto> SizeVariants { get; init; }
}
public record UpdateSizeVariantDto
{
    public Guid SizeVariantId { get; init; }
    public string Size { get; init; }
    public int Stock { get; init; }
    public StockLevel Level { get; init; }
}