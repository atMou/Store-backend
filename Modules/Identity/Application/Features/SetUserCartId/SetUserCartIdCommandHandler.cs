

using Shared.Application.Commands;

namespace Identity.Application.Features.SetUserCartId;

public record SetUserCartIdResponse();

public class SetUserCartIdCommandHandler(IOptions<JwtOptions> options, IClock clock, IdentityDbContext dbContext, IUserContext userContext)
    : ICommandHandler<SetUserCartIdCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(SetUserCartIdCommand command, CancellationToken cancellationToken)
    {
        var db =

            from _ in userContext.IsSameUser<IO>(UserId.From(command.UserId),
                UnAuthorizedError.New($"You are not Authorized to Set User Cart Id")).As()

            from user in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                await ctx.Users.FirstOrDefaultAsync(user => user.Id == UserId.From(command.UserId), e.Token))
            from _1 in when(user is null,
                IO.fail<Unit>(NotFoundError.New($"User with id: '{command.UserId}' does not exists"))).As()

            from userUpdated in IO.lift(() => user.SetCartId(CartId.From(command.CartId)))
            from _2 in Db<IdentityDbContext>.lift(ctx =>
                {
                    ctx.Users.Entry(user).CurrentValues.SetValues(userUpdated);
                    return unit;
                })
            select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}
