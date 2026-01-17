using Identity.Application.Features.DeleteAddress;

namespace Identity.Presentation.Requests;

public record DeleteAddressRequest
{
    public Guid UserId { get; init; }
    public Guid AddressId { get; init; }


    public DeleteAddressCommand ToCommand() =>
        new DeleteAddressCommand
        {
            UserId = Shared.Domain.ValueObjects.UserId.From(UserId),
            AddressId = Shared.Domain.ValueObjects.AddressId.From(AddressId),

        };
}