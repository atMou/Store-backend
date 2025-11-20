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

            from _ in userContext.IsSameUser<IO>(command.UserId,
                UnAuthorizedError.New($"You are not Authorized to Delete User Cart Id")).As()
            from user in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                await ctx.Users.FirstOrDefaultAsync(user => user.Id == command.UserId, e.Token))
            from _1 in when(user is null,
                IO.fail<Unit>(NotFoundError.New($"User with id: '{command.UserId}' does not exists"))).As()
            from a in when(user.CartId != command.CartId,
                IO.fail<Unit>(NotFoundError.New($"You do not have Cart Id: '{command.CartId}'"))).As()

            from userUpdated in IO.lift(() => user.DeleteCartId())
            from _2 in Db<IdentityDbContext>.lift(ctx =>
                {
                    ctx.Users.Entry(user).CurrentValues.SetValues(userUpdated);
                    return unit;
                })
            select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

