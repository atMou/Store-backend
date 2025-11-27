namespace Identity.Domain.Contracts;

public record UpdateAddressDto
{
    public Guid AddressId { get; init; }
    public string? City { get; init; }
    public string? Street { get; init; }
    public uint? PostalCode { get; init; }
    public bool IsMain { get; init; }
    public uint? HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }
}