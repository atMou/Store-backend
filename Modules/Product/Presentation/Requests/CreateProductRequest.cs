namespace Product.Presentation.Requests;

public class CreateProductRequest
{
    public string Slug { get; set; }
    public IFormFile[] Images { get; set; }
    public bool[] IsMain { get; set; }
    public decimal Price { get; set; }
    public decimal? NewPrice { get; set; }
    public string Type { get; set; }
    public string SubType { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string Description { get; set; }
    public List<CreateColorVariantRequest> ColorVariants { get; set; }
    public List<CreateAttributeRequest> DetailsAttributes { get; set; }
    public List<CreateAttributeRequest> SizeFitAttributes { get; set; }
    public List<CreateMaterialDetailRequest> MaterialDetails { get; set; }
}

public record CreateMaterialDetailRequest
{
    public string Material { get; init; }
    public decimal Percentage { get; init; }
    public string Detail { get; init; }
}

public class CreateAttributeRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class CreateColorVariantRequest
{
    public IFormFile[] Images { get; set; }
    public bool[] IsMain { get; set; }
    public string Color { get; set; }
}

