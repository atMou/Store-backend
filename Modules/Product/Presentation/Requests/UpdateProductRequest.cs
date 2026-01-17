using Product.Application.Features.UpdateProduct;

namespace Product.Presentation.Requests;

public record UpdateProductRequest
{
    public Guid ProductId { get; init; }
    public IFormFile[]? Images { get; init; }
    public bool[]? IsMain { get; set; }
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
    public IEnumerable<UpdateImageRequest> ImageDtos { get; init; }
    public IEnumerable<UpdateAttributeRequest> DetailsAttributes { get; init; }
    public IEnumerable<UpdateAttributeRequest> SizeFitAttributes { get; init; }
    public IEnumerable<UpdateMaterialDetailRequest> MaterialDetails { get; init; }
    public IEnumerable<Guid>? AlternativesIds { get; init; }
    public IEnumerable<UpdateVariantRequest> Variants { get; set; }


    public UpdateProductCommand ToCommand()
    {
        return new UpdateProductCommand(
            new UpdateProductDto()
            {
                ProductId = Shared.Domain.ValueObjects.ProductId.From(ProductId),
                ImageDtos = ImageDtos.Select(request => request.ToDto()),
                IsMain = IsMain ?? [],
                Slug = Slug,
                Brand = Brand,
                Category = Category,
                SubCategory = SubCategory,
                Type = Type,
                SubType = SubType,
                IsFeatured = IsFeatured,
                IsNew = IsNew,
                IsBestSeller = IsBestSeller,
                IsTrending = IsTrending,
                Price = Price,
                NewPrice = NewPrice,
                Description = Description,
                AlternativesIds = AlternativesIds is null ? [] : AlternativesIds.Select(guid => Shared.Domain.ValueObjects.ProductId.From(guid)),
                Variants = Variants.Select(request => request.ToDto()),
                Images = Images ?? [],
                DetailsAttributes = DetailsAttributes.Select(request => request.ToDto()),
                SizeFitAttributes = SizeFitAttributes.Select(request => request.ToDto()),
                MaterialDetails = MaterialDetails.Select(request => request.ToDto()),


            }
        );

    }
}

public record UpdateMaterialDetailRequest
{
    public string Material { get; init; }
    public decimal Percentage { get; init; }
    public string Detail { get; init; }

    public UpdateMaterialDetailDto ToDto() => new UpdateMaterialDetailDto
    {
        Material = Material,
        Percentage = Percentage,
        Detail = Detail
    };
}

public record UpdateAttributeRequest
{
    public string Name { get; set; }
    public string Description { get; set; }

    public UpdateAttributeDto ToDto() => new UpdateAttributeDto
    {
        Name = Name,
        Description = Description
    };
}

public record UpdateVariantRequest
{
    public Guid VariantId { get; init; }
    public bool[]? IsMain { get; set; }
    public IFormFile[]? Images { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }
    public int Stock { get; init; }
    public IEnumerable<UpdateImageRequest> ImageDtos { get; set; }

    public UpdateColorVariantDto ToDto() => new UpdateColorVariantDto
    {
        ColorVariantId = Shared.Domain.ValueObjects.ColorVariantId.From(VariantId),
        IsMain = IsMain ?? [],
        Images = Images ?? [],
        Color = Color,
        //Size = Size,
        //StockLow = StockLow,
        //StockMid = StockMid,
        //StockHigh = StockHigh,
        //Stock = Stock,
        ImageDtos = ImageDtos.Select(request => request.ToDto())
    };

}
public class UpdateImageRequest
{
    public Guid ProductImageId { get; set; }
    public string Url { get; set; }
    public string AltText { get; set; }
    public bool IsMain { get; set; }
    public bool IsDeleted { get; set; }

    public UpdateImageDto ToDto() => new UpdateImageDto
    {
        ImageId = ImageId.From(ProductImageId),
        Url = Url,
        AltText = AltText,
        IsMain = IsMain,
        IsDeleted = IsDeleted
    };

}

