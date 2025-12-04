namespace Product.Domain.Contracts;

public class UpdateProductImageDto
{
    public ProductImageId ProductImageId { get; init; }
    public string Url { get; init; }
    public string AltText { get; init; }
    public bool IsMain { get; init; }
    public ProductId ProductId { get; init; }

}
public class UpdateVariantImageDto
{
    public ProductImageId ProductImageId { get; init; }
    public string Url { get; init; }
    public string AltText { get; init; }
    public bool IsMain { get; init; }
    public VariantId VariantId { get; init; }

}
