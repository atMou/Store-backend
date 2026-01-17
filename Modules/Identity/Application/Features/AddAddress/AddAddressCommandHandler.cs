using Identity.Application.Contracts;

using Address = Identity.Domain.Models.Address;

namespace Identity.Application.Features.AddAddress;

public record AddAddressCommand : ICommand<Fin<UserResult>>
{

    public string Fullname { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string? ExtraDetails { get; init; }
    public int HouseNumber { get; init; }
    public int PostalCode { get; init; }
    public bool? IsMain { get; init; } = false;

}


public class AddAddressCommandHandler(
    IUserContext userContext,
    IOptions<JwtOptions> options,
    IClock clock,
    IdentityDbContext dbContext)
    : ICommandHandler<AddAddressCommand, Fin<UserResult>>
{
    public async Task<Fin<UserResult>> Handle(AddAddressCommand command, CancellationToken cancellationToken)
    {
        var db = from userResult in userContext.GetCurrentUser<IO>().As()
                 from u in GetUpdateEntity<IdentityDbContext, User>(
                     user => user.Id == UserId.From(userResult.Id),
                     NotFoundError.New($"User with id: '{userResult.Id}' does not exists"),
                     null,
                     user => user.AddAddress(Address.Create(command.Fullname, command.Street, command.City, (uint)command.PostalCode, command.IsMain ?? false, command.ExtraDetails, (uint)command.HouseNumber))
                 )

                 select u.ToResult();

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

