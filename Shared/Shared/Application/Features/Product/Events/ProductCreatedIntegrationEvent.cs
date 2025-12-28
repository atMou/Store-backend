using Shared.Application.Contracts.Product.Dtos;

namespace Shared.Application.Features.Product.Events;

public record ProductCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; init; }
    public string Brand { get; init; }
    public string Slug { get; init; }
    public IEnumerable<VariantDto> Variants { get; init; }


}