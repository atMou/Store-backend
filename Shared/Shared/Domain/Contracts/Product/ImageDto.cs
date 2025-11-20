namespace Shared.Domain.Contracts.Product;

public record ImageDto
{
    public Guid Id { get; set; }
    public string Url { get; init; }
    public string AltText { get; init; }
    public bool IsMain { get; init; }
}
