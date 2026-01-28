using Identity.Application.Contracts;
using Identity.Infrastructure.Authentication;

namespace Identity.Application.Features.Login;

public record LoginCommand(string Email, string Password) : ICommand<Fin<LoginResult>>;

public record LoginResult(UserResult User, RefreshToken RefreshToken, string AccessToken);

public record LoginResponse(UserResult User, string AccessToken);

public class LoginCommandHandler(
    IOptions<JwtOptions> options,
    IJwtProvider jwtProvider,
    IClock clock,
    IdentityDbContext dbContext) : ICommandHandler<LoginCommand, Fin<LoginResult>>
{
    public async Task<Fin<LoginResult>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var db = from email in Email.From(command.Email)
                 from u in GetEntity<IdentityDbContext, User>(
                     user => user.Email == email,
                     NotFoundError.New("Invalid credentials."),
                     opt =>
                     {
                         opt = opt.AddInclude(user => user.RefreshTokens);
                         opt = opt.AddInclude(user => user.LikedProducts);
                         opt.AsSplitQuery = true;
                         return opt;
                     })
                 from _1 in IsVerified(u)
                 from _2 in UpdateEntity<IdentityDbContext, User>(u,
                     u => u.VerifyPassword(command.Password)
                         .Map(user => user.RevokeTokens(clock.UtcNow, "New refresh token generated")))
                 let accessToken = jwtProvider.Generate(u, options.Value)
                 let refreshToken = RefreshToken.Generate(u.Id, options.Value.Salt, clock.UtcNow)
                 let user = u.AddRefreshToken(refreshToken, clock.UtcNow)
                 select new LoginResult(user.ToResult(), refreshToken, accessToken);


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Fin<Unit> IsVerified(User user)
    {
        if (user.IsEmailVerified || user.IsPhoneVerified)
        {
            return unit;
        }

        return FinFail<Unit>(UnAuthorizedError.New("Unauthorized access. Please verify your email or your phone to continue."));
    }
}