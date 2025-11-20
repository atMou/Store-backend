namespace Identity.Application.Features.DeleteRole;

public record DeleteRoleCommand : ICommand<Fin<Unit>>
{
    public Guid UserId { get; init; }
    public string Role { get; init; }
}


public class DeleteRoleCommandHandler(IdentityDbContext dbContext)
    : ICommandHandler<DeleteRoleCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var db =
            from user in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                await ctx.Users.FirstOrDefaultAsync(user => user.Id == UserId.From(command.UserId), e.Token))
            from _1 in when(user is null,
                IO.fail<Unit>(NotFoundError.New($"User with id: '{command.UserId}' does not exists"))).As()

            from userUpdated in IO.lift(() => user.DeleteRoles(command.Role))
            from _2 in Db<IdentityDbContext>.lift(ctx =>
                {
                    ctx.Users.Entry(user).CurrentValues.SetValues(userUpdated);
                    return unit;
                })
            select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

