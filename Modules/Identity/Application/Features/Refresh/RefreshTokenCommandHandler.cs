using Identity.Infrastructure.Authentication;

namespace Identity.Application.Features.Refresh;

public record RefreshTokenCommand(string? RefreshToken, string Email) : ICommand<Fin<RefreshCommandResult>>;

public record RefreshCommandResult(RefreshToken RefreshToken, string AccessToken);

public record RefreshTokenResponse(string AccessToken)
{
}
public class RefreshTokenCommandHandler(IClock clock, IOptions<JwtOptions> options, IJwtProvider jwtProvider, IdentityDbContext dbContext)
    : ICommandHandler<RefreshTokenCommand, Fin<RefreshCommandResult>>
{
    public Task<Fin<RefreshCommandResult>> Handle(RefreshTokenCommand tokenCommand, CancellationToken cancellationToken)
    {
        var db =
            from t in (GetUserByEmail(tokenCommand.Email),
                    GetRefreshToken(tokenCommand.RefreshToken))
                .Apply(((user, token) => (user, token)))

            let _ = t.token.Revoke("New Refresh token generated.", clock.UtcNow)
            let newRefreshToken = RefreshToken.Generate(t.user.Id, options.Value.Salt, clock.UtcNow)
            let newAccessToken = jwtProvider.Generate(t.user, options.Value)
            from _2 in Db<IdentityDbContext>.lift(cxt => cxt.RefreshTokens.Add(newRefreshToken))
            select new RefreshCommandResult(newRefreshToken, newAccessToken);
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


    private Db<IdentityDbContext, RefreshToken> GetRefreshToken(string? refreshToken)
    {
        return from token in string.IsNullOrEmpty(refreshToken)
                ? FinFail<string>(UnAuthorizedError.New("You are not authorized."))
                : refreshToken
               let hashedToken = Helpers.Hash(token, options.Value.Salt)
               from existingRefreshToken in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                   await ctx.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hashedToken, e.Token))
               from _1 in when(existingRefreshToken is null || existingRefreshToken.IsRevoked,
                   IO.fail<Unit>(UnAuthorizedError.New("You are not authorized.")))
               select existingRefreshToken;
    }

    private Db<IdentityDbContext, User> GetUserByEmail(string email)
    {
        return from user in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
            await ctx.Users.FirstOrDefaultAsync(u => u.Email == Email.FromUnsafe(email), e.Token))
               from _1 in when(user is null, IO.fail<Unit>(UnAuthorizedError.New("Invalid credentials")))
               select user;
    }
}