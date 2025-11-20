
using Identity.Infrastructure.Authentication;

namespace Identity.Application.Features.EmailVerification;

public record EmailVerificationCommand(string Email, string Token) : ICommand<Fin<EmailVerificationCommandResult>>;

public record EmailVerificationCommandResult(RefreshToken RefreshToken, string AccessToken);
public record EmailVerificationResponse(string AccessToken);

internal class EmailVerificationCommandHandler(
    IJwtProvider jwtProvider,
    IdentityDbContext dbContext,
    IPublishEndpoint endpoint,
    IClock clock,
    IOptions<JwtOptions> options
) : ICommandHandler<EmailVerificationCommand, Fin<EmailVerificationCommandResult>>
{
    public Task<Fin<EmailVerificationCommandResult>> Handle(EmailVerificationCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from g in ValidateGuidToken(command.Token)

            from user in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                await ctx.Users.FirstOrDefaultAsync(user => user.Email == Email.FromUnsafe(command.Email), e.Token))

            from _1 in when(user is null,
                IO.fail<Unit>(UnAuthorizedError.New("Please VerifyEmail your email and try again.")))

            from _2 in when(user.IsEmailVerified, IO.fail<Unit>(UnAuthorizedError.New("Email is already verified.")))

            from _4 in user.VerifyEmail(g, user.EmailConfirmationExpiresAt ?? clock.UtcNow)

            let accessToken = jwtProvider.Generate(user, options.Value, TimeSpan.FromMinutes(30))
            let refreshToken = RefreshToken.Generate(user.Id, options.Value.Salt, clock.UtcNow)

            select (user, accessToken, refreshToken);

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async tuple =>
                {
                    await endpoint.Publish(new UserEmailVerifiedIntegrationEvent(tuple.user.Id.Value),
                        cancellationToken);
                    return new EmailVerificationCommandResult(tuple.refreshToken, tuple.accessToken);
                }
            );
    }


    private Fin<Guid> ValidateGuidToken(string repr)
    {
        return Guid.TryParse(repr, out var token)
            ? FinSucc(token)
            : FinFail<Guid>(ValidationError.New("Invalid verification RefreshToken format."));
    }
}
