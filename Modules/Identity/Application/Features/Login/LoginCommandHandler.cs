using Identity.Infrastructure.Authentication;

namespace Identity.Application.Features.Login;


public record LoginCommand(string Email, string Password) : ICommand<Fin<LoginCommandResult>>;

public record LoginCommandResult(RefreshToken RefreshToken, string AccessToken);
public record LoginResponse(string AccessToken);


public class LogoutCommandHandler(IOptions<JwtOptions> options,
    IJwtProvider jwtProvider,
    IClock clock,
    IdentityDbContext dbContext) : ICommandHandler<LoginCommand, Fin<LoginCommandResult>>
{
    public Task<Fin<LoginCommandResult>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var db = from email in Email.From(command.Email)

                 from u in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                    await ctx.Users.FirstOrDefaultAsync(user => user.Email == email, e.Token))
                 from _1 in when(u is null, IO.fail<Unit>(NotFoundError.New($"Invalid credentials.")))
                 from _2 in u.VerifyPassword(command.Password).MapFail(_ => NotFoundError.New($"Invalid credentials."))
                 let accessToken = jwtProvider.Generate(u, options.Value)
                 let refreshToken = RefreshToken.Generate(u.Id, options.Value.Salt, clock.UtcNow)
                 from _3 in GetAndRevokeOldRefreshTokens(u.Id)
                 from _4 in Db<IdentityDbContext>.lift((ctx) =>
                     ctx.RefreshTokens.Add(refreshToken))
                 select new LoginCommandResult(refreshToken, accessToken);
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Db<IdentityDbContext, Unit> GetAndRevokeOldRefreshTokens(UserId userId)
    {
        return from rts in Db<IdentityDbContext>.liftIO(async
                   (ctx, e) => await ctx.RefreshTokens
                    .Where(token => token.UserId == userId && !token.IsRevoked)
                       .ToListAsync(e.Token))
               let _ = rts.AsIterable().Iter(token => token.Revoke("New RefreshToken created", clock.UtcNow))
               select unit;
    }
}
