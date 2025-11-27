namespace Shared.Application.Contracts.User.Results;

public record AddressResult
{
    public string Street { get; init; }
    public string City { get; init; }
    public uint PostalCode { get; init; }
    public uint HouseNumber { get; set; }
    public bool IsMain { get; init; }
    public string? ExtraDetails { get; init; }
}