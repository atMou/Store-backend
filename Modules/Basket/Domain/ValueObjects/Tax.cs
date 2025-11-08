namespace Basket.Basket.Domain.ValueObjects;

public record Tax
{
    public decimal Value { get; set; }
    public decimal Rate => Value / 100m;
}