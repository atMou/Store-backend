namespace Identity.Domain.Models;

public record Address : Entity<AddressId>
{
    private Address() : base(AddressId.New)
    {
    }


    public static Address Create(
        string street,
        string city,
        uint postalCode,
        bool isMain,
        string? extraDetails,
        uint houseNumber)
    {
        return new Address
        {
            Street = street,
            City = city,
            PostalCode = postalCode,
            IsMain = isMain,
            ExtraDetails = extraDetails,
            HouseNumber = houseNumber
        };
    }


    public UserId UserId { get; init; }
    public string Street { get; init; }
    public uint HouseNumber { get; init; }
    public string City { get; init; }
    public uint PostalCode { get; init; }
    public bool IsMain { get; init; }
    public string? ExtraDetails { get; init; }

    public Address Update(string? street, string? city, uint? postalCode, bool isMain, string? extraDetails, uint? houseNumber)
    {
        return this with
        {
            Street = street ?? Street,
            City = city ?? City,
            PostalCode = postalCode ?? PostalCode,
            IsMain = isMain,
            ExtraDetails = extraDetails ?? ExtraDetails,
            HouseNumber = houseNumber ?? HouseNumber
        };
    }
}