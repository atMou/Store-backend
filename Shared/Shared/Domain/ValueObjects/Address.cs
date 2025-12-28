namespace Shared.Domain.ValueObjects;

public record Address
{
	public string City { get; init; }
	public string Street { get; init; }
	public uint PostalCode { get; init; }
	public uint HouseNumber { get; init; }
	public string? ExtraDetails { get; init; }


}
