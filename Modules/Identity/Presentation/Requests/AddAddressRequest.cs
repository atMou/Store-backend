using Identity.Application.Features.AddAddress;

namespace Identity.Presentation.Requests;

public record AddAddressRequest
{
    public string Fullname { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string? ExtraDetails { get; init; }
    public int HouseNumber { get; init; }
    public int PostalCode { get; init; }
    public bool? IsMain { get; init; } = false;

    public AddAddressCommand ToCommand()
    {
        return new AddAddressCommand
        {
            Fullname = Fullname,
            Street = Street,
            City = City,
            ExtraDetails = ExtraDetails,
            HouseNumber = HouseNumber,
            PostalCode = PostalCode,
            IsMain = IsMain
        };
    }
}