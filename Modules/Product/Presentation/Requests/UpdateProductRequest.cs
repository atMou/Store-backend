using Product.Application.Features.UpdateProduct;

namespace Product.Presentation.Requests;

public record UpdateProductRequest
{
    public ProductId ProductId { get; init; } = null!;
    public IFormFile[] Images { get; init; }
    public bool[] IsMain { get; set; }
    public string Slug { get; init; }
    public string Brand { get; init; }
    public string Category { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsNew { get; init; }
    public bool IsBestSeller { get; init; }
    public bool IsTrending { get; init; }
    public decimal Price { get; init; }
    public decimal NewPrice { get; init; }
    public string Description { get; init; }
    public IEnumerable<UpdateProductImageDto> ImageDtos { get; init; } = [];
    public IEnumerable<ProductId> AlternativesIds { get; init; }
    public IEnumerable<UpdateVariantDto> Variants { get; set; }


    public UpdateProductCommand ToCommand()
    {
        return new UpdateProductCommand(
            new UpdateProductDto()
            {
                ProductId = ProductId,
                ImageDtos = ImageDtos,
                IsMain = IsMain,
                Slug = Slug,
                Brand = Brand,
                Category = Category,
                IsFeatured = IsFeatured,
                IsNew = IsNew,
                IsBestSeller = IsBestSeller,
                IsTrending = IsTrending,
                Price = Price,
                NewPrice = NewPrice,
                Description = Description,
                AlternativesIds = AlternativesIds,
                Variants = Variants,



            }
        );

    }
}