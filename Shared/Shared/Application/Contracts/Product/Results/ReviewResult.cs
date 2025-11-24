namespace Shared.Application.Contracts.Product.Results;

public record ReviewResult
{
    public Guid Id { get; init; }
    public string Comment { get; init; } = null!;
    public double Rating { get; init; }
    public string RatingDescription { get; init; } = null!;
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid ProductId { get; set; }
}