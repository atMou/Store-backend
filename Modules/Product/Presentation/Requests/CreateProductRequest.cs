using Product.Application.Features.CreateProduct;

namespace Product.Presentation.Requests;

public record CreateProductRequest
{
    public string Slug { get; init; } = null!;
    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public string Brand { get; init; } = null!;

    public string Category { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public string Description { get; init; } = null!;

    public IEnumerable<CreateVariantRequest> Variants { get; set; } = [];


    public CreateProductCommand ToCommand()
    {
        return new CreateProductCommand
        {
            Slug = Slug,
            Images = Images,
            IsMain = IsMain,
            Price = Price,
            NewPrice = NewPrice,
            Brand = Brand,
            Category = Category,
            Description = Description,
            Variants = Variants.Select(v => v.ToCommand())


        };
    }
}