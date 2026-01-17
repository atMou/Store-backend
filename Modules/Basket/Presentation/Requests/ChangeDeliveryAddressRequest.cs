using Basket.Application.Features.Cart.ChangeDeliveryAddress;

namespace Basket.Presentation.Requests;

public record ChangeDeliveryAddressRequest
{
    public Guid UserId { get; set; }
    public Guid CartId { get; init; }
    public Guid AddressId { get; init; }


    public ChangeDeliveryAddressCommand ToCommand()
    {
        return new ChangeDeliveryAddressCommand
        {
            CartId = Shared.Domain.ValueObjects.CartId.From(CartId),
            AddressId = AddressId,
            UserId = Shared.Domain.ValueObjects.UserId.From(UserId)
        };
    }
}