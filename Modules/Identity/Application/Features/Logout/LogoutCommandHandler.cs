using Shared.Infrastructure.Clock;

namespace Identity.Application.Features.Logout;


public record LogoutCommand(string Email) : ICommand<Fin<LogoutCommandResult>>;

public record LogoutCommandResult();

public record LogoutResponse();
public class LogoutCommandHandler(IClock clock, IdentityDbContext dbContext) : ICommandHandler<LogoutCommand, Fin<LogoutCommandResult>>
{
    public Task<Fin<LogoutCommandResult>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var db = from u in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                await ctx.Users.FirstOrDefaultAsync(user => user.Email == Email.FromUnsafe(command.Email), e.Token))
                 from _1 in when(u is null, IO.fail<Unit>(
                     NotFoundError.New($"User with Email: '{command.Email}' is not available")))

                 from _2 in GetAndRevokeOldRefreshTokens(u.Id)
                 select new LogoutCommandResult();
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Db<IdentityDbContext, Unit> GetAndRevokeOldRefreshTokens(UserId userId)
    {
        return from rts in Db<IdentityDbContext>.liftIO(async
                   (ctx, e) => await ctx.RefreshTokens.Where(token => token.UserId == userId && !token.IsRevoked)
                       .ToListAsync(e.Token))
               let _ = rts.AsIterable().Iter(token => token.Revoke("Logout Action", clock.UtcNow))
               select unit;

    }
}
