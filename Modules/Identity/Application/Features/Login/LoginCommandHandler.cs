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
                 from u in GetEntity<IdentityDbContext, User>(
                     user => user.Email == email,
                     opt =>
                     {
                         opt.AddInclude(user => user.RefreshTokens);
                         opt.AsSplitQuery = true;
                         return opt;
                     },
                     NotFoundError.New($"Invalid credentials."))


                 from _ in UpdateEntity<IdentityDbContext, User>(u,
                      u => u.VerifyPassword(command.Password),
                      user => user.RevokeTokens(clock.UtcNow, "New refresh token generated"))
                 let accessToken = jwtProvider.Generate(u, options.Value)
                 let refreshToken = RefreshToken.Generate(u.Id, options.Value.Salt, clock.UtcNow)
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
