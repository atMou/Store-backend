
namespace Identity.Application.Features.Register;

public record RegisterCommand(
    CreateUserDto CreateUserDto) : ICommand<Fin<RegisterCommandResult>>;

public record RegisterCommandResult(string Message);

internal class RegisterCommandHandler(
    IdentityDbContext dbContext,
    IUserRepository userRepository,
    IPublishEndpoint endpoint,
    IImageService imageService
) : ICommandHandler<RegisterCommand, Fin<RegisterCommandResult>>
{
    public async Task<Fin<RegisterCommandResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var db = from user in User.Create(command.CreateUserDto)
                 from _ in Db<IdentityDbContext>.liftIO(ctx => userRepository.CheckIfUserExists(user.Email, ctx))

                 from image in Optional(command.CreateUserDto.Image).Match<IO<ImageUrl>>(file =>
                        imageService.UploadImage(file, user.Id.ToString()), () => IO.pure<ImageUrl>(null!))
                 let _0 = user.SetAvatar(image)
                 let _1 = user.HashPassword(user.Password)
                 from _2 in Db<IdentityDbContext>.liftIO(ctx => userRepository.AddUser(user, ctx))
                 select user;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(u =>
            {
                endpoint.Publish(new UserCreatedIntegrationEvent(u.Email.Value, u.EmailConfirmationToken, u.EmailConfirmationExpiresAt), cancellationToken);
                return new RegisterCommandResult($"Please check your email: '{u.Email.Value}' to confirm registration");
            });

    }
}