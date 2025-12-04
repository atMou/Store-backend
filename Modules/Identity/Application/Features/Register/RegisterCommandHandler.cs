
using Db.Errors;

namespace Identity.Application.Features.Register;

public record RegisterCommand(
    CreateUserDto CreateUserDto) : ICommand<Fin<RegisterResult>>;

public record RegisterResult(string Message);

internal class RegisterCommandHandler(
    IdentityDbContext dbContext,
    IPublishEndpoint endpoint,
    IImageService imageService
) : ICommandHandler<RegisterCommand, Fin<RegisterResult>>
{
    public async Task<Fin<RegisterResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CreateUserDto;
        var db =
            from img in dto.Image.IsNotNull()
                ? imageService.UploadImage(dto.Image!, $"{dto.FirstName}_{dto.LastName}")
                : IO.pure(ImageUrl.FromUnsafe(""))
            from res in AddEntityIfNotExists<
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
                return new RegisterResult($"Please check your email: '{u.Email.Value}' to confirm registration");
            });

    }
}