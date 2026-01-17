namespace Shared.Application.Features.Product.Events;

public record ProductCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; init; }
    public string Brand { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public IEnumerable<CreateColorVariantDto> ColorVariants { get; set; }
}


public record CreateColorVariantDto
{
    public Guid ColorVariantId { get; init; }
    public string Color { get; init; } = null!;
}

