using Basket.Application.Features.Cart.ChangeDeliveryAddress;

namespace Basket.Presentation.Requests;

public record ChangeDeliveryAddressRequest
{

	public CartId CartId { get; init; }
	public string Street { get; init; }
	public string City { get; init; }
	public uint PostalCode { get; init; }
	public uint HouseNumber { get; init; }
	public string ExtraDetails { get; init; }

	public ChangeDeliveryAddressCommand ToCommand()
	{

		return new ChangeDeliveryAddressCommand
		{
			CartId = CartId,
			Street = Street,
			City = City,
			PostalCode = PostalCode,
			HouseNumber = HouseNumber,
			ExtraDetails = ExtraDetails
		};
	}
}