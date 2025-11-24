namespace Identity.Application.Features.DeleteUserCartId;
public record DeleteUserCartIdCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public CartId CartId { get; init; }
}


public class DeleteUserCartIdCommandHandler(IdentityDbContext dbContext, IUserContext userContext)
    : ICommandHandler<DeleteUserCartIdCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeleteUserCartIdCommand command, CancellationToken cancellationToken)
    {
        var db =
            from _ in EnsureIsSameUserOrAdmin(command.UserId)
            from user in GetUpdateEntityA<IdentityDbContext, User>(
                user => user.Id == command.UserId,
                NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
                null,
                user => user.DeleteCartId())

            from a in when(user.CartId != command.CartId,
                IO.fail<Unit>(NotFoundError.New($"You do not have Cart Id: '{command.CartId}'"))).As()
            select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private IO<Unit> EnsureIsSameUserOrAdmin(UserId userId) =>
        userContext.IsSameUser<IO>(userId, UnAuthorizedError.New($"You are not Authorized to Delete User Cart Id"))
        | userContext.IsInRole<IO>(Role.Admin, UnAuthorizedError.New($"You are not Authorized to Delete User Cart Id"))
            .As();
}

