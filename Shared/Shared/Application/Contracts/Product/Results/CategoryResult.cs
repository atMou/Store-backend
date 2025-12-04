namespace Shared.Application.Contracts.Product.Results;

public record CategoryResult
{

    public string Name { get; init; }
    public IEnumerable<CategoryResult> Subcategories { get; init; }


}