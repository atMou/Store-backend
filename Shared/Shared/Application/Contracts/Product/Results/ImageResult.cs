namespace Shared.Application.Contracts.Product.Results;

public record ImageResult
{
    public Guid Id { get; set; }
    public string Url { get; init; }
    public string AltText { get; init; }
    public bool IsMain { get; init; }
}
