namespace Shared.Domain.ValueObjects;

public record Address(string City, string Street, uint PostalCode, uint HouseNumber, string? ExtraDetails)
{
}
