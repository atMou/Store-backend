using Shared.Domain.ValueObjects;

namespace Basket.Basket.Domain.ValueObjects;

public record Shipping
{
    public Money Cost { get; set; }
    public Address Address { get; set; }

}

public record Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
}