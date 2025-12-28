using Identity.Application.Features.ToggleLikedProducts;

namespace Identity.Presentation.Requests;

public record ToggleLikeRequest
{
    public IEnumerable<Guid> ProductIds { get; set; }

    public ToggleLikedProductsCommand ToCommand()
    {

        return new ToggleLikedProductsCommand
        {
            ProductIds = ProductIds.Select(id => ProductId.From(id)).ToList()
        };
    }
}