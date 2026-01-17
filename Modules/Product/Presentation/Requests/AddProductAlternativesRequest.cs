using Product.Application.Features.AddProductAlternatives;


namespace Product.Presentation.Requests;

public record AddProductAlternativesRequest
{

    public Guid ProductId { get; init; }

    public IEnumerable<Guid> AlternativeProductIds { get; init; } = [];

    public AddProductAlternativesCommand ToCommand()
    {
        return new AddProductAlternativesCommand
        {
            ProductId = Shared.Domain.ValueObjects.ProductId.From(ProductId),
            AlternativeProductIds = AlternativeProductIds.Select(Shared.Domain.ValueObjects.ProductId.From)
        };
    }
}
