namespace Shared.Application.Contracts.Product.Dtos;

public class VariantDto
{
    public Guid VariantId { get; init; }
    public string Sku { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
    public int Stock { get; init; }
    public int Low { get; init; }
    public int High { get; init; }
    public int Mid { get; init; }
}
