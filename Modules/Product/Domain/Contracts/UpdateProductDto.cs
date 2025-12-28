namespace Product.Domain.Contracts;

public record UpdateProductDto
{
    public ProductId ProductId { get; init; }
    public bool[] IsMain { get; init; }
    public IFormFile[] Images { get; init; }
    public string Slug { get; init; }
    public string Brand { get; init; }
    public string Category { get; init; }
    public string SubCategory { get; set; }
    public string Type { get; set; }
    public string SubType { get; set; }
    public bool IsFeatured { get; init; }
    public bool IsNew { get; init; }
    public bool IsBestSeller { get; init; }
    public bool IsTrending { get; init; }
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public string Description { get; init; }
    public IEnumerable<UpdateImageDto> ImageDtos { get; set; }
    public IEnumerable<UpdateVariantDto> Variants { get; set; }
    public IEnumerable<ProductId> AlternativesIds { get; set; }
    public IEnumerable<UpdateAttributeDto> DetailsAttributes { get; set; }
    public IEnumerable<UpdateAttributeDto> SizeFitAttributes { get; set; }

    public IEnumerable<UpdateMaterialDetailDto> MaterialDetails { get; set; }
}

public record UpdateMaterialDetailDto
{
    public string Material { get; init; }
    public decimal Percentage { get; init; }
    public string Detail { get; init; }
}

public record UpdateAttributeDto
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public record UpdateVariantDto
{
    public VariantId VariantId { get; init; }
    public bool[] IsMain { get; set; }
    public IFormFile[] Images { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }
    public int Stock { get; init; }
    public IEnumerable<UpdateImageDto> ImageDtos { get; set; }

}
public class UpdateImageDto
{
    public ImageId ImageId { get; set; }
    public string Url { get; set; }
    public string AltText { get; set; }
    public bool IsMain { get; set; }
    public bool IsDeleted { get; set; }

}

