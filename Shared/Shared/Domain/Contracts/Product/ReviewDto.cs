namespace Shared.Domain.Contracts.Product;

public record ReviewDto
{
    public Guid Id { get; init; }
    public string Comment { get; init; } = null!;
    public double Rating { get; init; }
    public string RatingDescription { get; init; } = null!;
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid ProductId { get; set; }
}