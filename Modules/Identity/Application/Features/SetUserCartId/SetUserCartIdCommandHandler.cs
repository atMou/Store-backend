
namespace Identity.Application.Features.SetUserCartId;
public record SetUserCartIdCommand(Guid UserId, Guid CartId) : ICommand<Fin<Unit>>;
public class SetUserCartIdCommandHandler(IdentityDbContext dbContext)
    : ICommandHandler<SetUserCartIdCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(SetUserCartIdCommand command, CancellationToken cancellationToken)
    {
        var db = GetUpdateEntity<IdentityDbContext, User>(
            user => user.Id == UserId.From(command.UserId),
                NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
              null,
                user => user.AddCartId(CartId.From(command.CartId))
            ).Map(_ => unit);

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}
