

namespace Identity.Application.Features.DeleteUser;

public record DeleteUserCommand(Guid UserId) : ICommand<Fin<DeleteUserCommandResult>>;

public record DeleteUserCommandResult();
public record DeleteUserResponse();

public class SetUserCartIdCommandHandler(IOptions<JwtOptions> options, IClock clock, IdentityDbContext dbContext, IUserContext userContext)
    : ICommandHandler<DeleteUserCommand, Fin<DeleteUserCommandResult>>
{
    public Task<Fin<DeleteUserCommandResult>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var db =

            from _ in userContext.IsSameUser<IO>(UserId.From(command.UserId), UnAuthorizedError.New($"You are not Authorized to Delete Users")).As()

            from user in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                await ctx.Users.FirstOrDefaultAsync(user => user.Id == UserId.From(command.UserId), e.Token))
            from _1 in when(user is null,
                IO.fail<Unit>(NotFoundError.New($"User with id: '{command.UserId}' does not exists"))).As()

                ///// If User has Pending orders, we cannot delete the user   //Todo: Implement this check when Orders module is ready

            from _2 in Db<IdentityDbContext>.lift((ctx) =>
            {
                ctx.Users.Remove(user);
                return unit;
            })

            select new DeleteUserCommandResult();
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}
