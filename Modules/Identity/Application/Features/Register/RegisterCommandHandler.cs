
using Db.Errors;

using Identity.Application.Events;

namespace Identity.Application.Features.Register;

public record RegisterCommand(
    CreateUserDto CreateUserDto) : ICommand<Fin<RegisterResult>>;

public record RegisterResult(string Message);

internal class RegisterCommandHandler(
    IdentityDbContext dbContext,
    IPublishEndpoint endpoint,
    IImageService imageService,
    IClock clock
) : ICommandHandler<RegisterCommand, Fin<RegisterResult>>
{
    public async Task<Fin<RegisterResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CreateUserDto;
        var db =
            from img in dto.Avatar.IsNotNull()
                ? imageService.UploadImage(dto.Avatar!, $"{dto.FirstName}_{dto.LastName}")
                : IO.pure(ImageUrl.FromUnsafe("", ""))
            from res in AddEntityIfNotExists<
                IdentityDbContext,
                User,
                CreateUserDto
            >(user => user.Email == Email.FromUnsafe(dto.Email),
                ConflictError.New($"User with email '{dto.Email}' already exists."),
                dto,
               _dto => User.Create(_dto, clock.UtcNow),
                user => user.HashPassword(user.Password),
                user => user.AssignUserToRoles(Role.Customer),
                user => user.AddAvatar(img))
            select res;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async u =>
            {
                await endpoint.Publish(new UserCreatedIntegrationEvent($"{u.FirstName.Value} {u.LastName.Value}", u.Email.Value, u.EmailConfirmationCode, u.EmailConfirmationToken, u.EmailConfirmationExpiresAt), cancellationToken);
                return new RegisterResult($"Please check your email: '{u.Email.Value}' to confirm registration");
            });

    }
}