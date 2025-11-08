namespace Shared.Domain.ValueObjects;

public record Address(string City, string Street, uint ZipCode, short HouseNumber, string? ExtraDetails)
{
}
