namespace Shared.Domain.ValueObjects;

public record Address(string City, string Street, uint ZipCode, uint HouseNumber, string? ExtraDetails)
{
}
