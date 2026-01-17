namespace Identity.Application.Features.DeleteAddress;

public record DeleteAddressCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public AddressId AddressId { get; init; }


}

public class DeleteAddressCommandHandler(
    IUserContext userContext,
    IdentityDbContext dbContext,
    IPublishEndpoint endpoint)
    : ICommandHandler<DeleteAddressCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
    {
        var db = from _ in userContext
                .IsSameUserF<Fin>(command.UserId, UnAuthorizedError.New("You can only delete your own addresses")).As()

                 from user in GetUpdateEntity<IdentityDbContext, User>(
                     user => user.Id == command.UserId,
                     NotFoundError.New($"User with id: '{command.UserId}' does not exist"),
                     null,
                     user => user.RemoveAddress(command.AddressId))

                 select unit;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
