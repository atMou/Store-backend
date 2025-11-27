
using Db.Errors;

namespace Identity.Application.Features.Register;

public record RegisterCommand(
    CreateUserDto CreateUserDto) : ICommand<Fin<RegisterCommandResult>>;

public record RegisterCommandResult(string Message);

internal class RegisterCommandHandler(
    IdentityDbContext dbContext,
    IPublishEndpoint endpoint,
    IImageService imageService
) : ICommandHandler<RegisterCommand, Fin<RegisterCommandResult>>
{
    public async Task<Fin<RegisterCommandResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CreateUserDto;
        var db =
            from img in dto.Image.IsNotNull()
                ? imageService.UploadImage(dto.Image!, $"{dto.FirstName}_{dto.LastName}")
                : IO.pure(ImageUrl.FromUnsafe(""))
            from res in AddEntity<
                IdentityDbContext,
                User,
                CreateUserDto
            >(user => user.Email == dto.Email,
                ConflictError.New("User with email '{dto.Email}' already exists."),
                dto,
                User.Create,
                user => user.HashPassword(user.Password),
                user => user.AddAvatar(img))
            select res;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(u =>
            {
                endpoint.Publish(new UserCreatedIntegrationEvent(u.Email.Value, u.EmailConfirmationToken, u.EmailConfirmationExpiresAt), cancellationToken);
                return new RegisterCommandResult($"Please check your email: '{u.Email.Value}' to confirm registration");
            });

    }
}