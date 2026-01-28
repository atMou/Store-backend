using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace Shared.Infrastructure.Authentication;

public record CurrentUser(Guid Id, string Email, string Name, bool HasPendingOrders, string AvatarUrl)
{
}

public interface IUserContext
{
    K<M, CurrentUser> GetCurrentUser<M>() where M : MonadIO<M>, Fallible<M>;
    K<M, CurrentUser> GetCurrentUserF<M>() where M : Monad<M>, Fallible<M>;
    K<M, Unit> HasRole<M>(Role role, Error error) where M : MonadIO<M>, Fallible<M>;
    K<M, Unit> HasRoleF<M>(Role role, Error error) where M : Monad<M>, Fallible<M>;
    K<M, Unit> IsSameUser<M>(UserId userId, Error error) where M : MonadIO<M>, Fallible<M>;
    K<M, Unit> IsSameUserF<M>(UserId userId, Error error) where M : Monad<M>, Fallible<M>;
    K<M, Unit> HasPermission<M>(Permission permission, Error error) where M : MonadIO<M>, Fallible<M>;
    K<M, Unit> HasPermissionF<M>(Permission permission, Error error) where M : Monad<M>, Fallible<M>;
}

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public K<M, CurrentUser> GetCurrentUser<M>() where M : MonadIO<M>, Fallible<M>
    {
        return from user in iff(User.IsNull(), M.Fail<ClaimsPrincipal>(UnAuthorizedError.New("You are not authorized")),
                M.Pure(User)!)
               let result = (ParseId(user), GetEmail(user), GetName(user), GetHasPendingOrders(user), GetAvatarUrl(user))
                   .Apply((id, email, name, hasPending, avatarUrl) => new CurrentUser(id, email, name, hasPending, avatarUrl)).As()
               from a in result.Match(M.Pure, M.Fail<CurrentUser>)
               select a;
    }
    public K<M, CurrentUser> GetCurrentUserF<M>() where M : Monad<M>, Fallible<M>
    {
        return from user in iff(User.IsNull(), M.Fail<ClaimsPrincipal>(UnAuthorizedError.New("You are not authorized")),
                M.Pure(User)!)
               let result = (ParseId(user), GetEmail(user), GetName(user), GetHasPendingOrders(user), GetAvatarUrl(user))
                   .Apply((id, email, name, hasPending, avatarUrl) => new CurrentUser(id, email, name, hasPending, avatarUrl)).As()
               from a in result.Match(M.Pure, M.Fail<CurrentUser>)
               select a;
    }


    public K<M, Unit> HasRole<M>(Role role, Error error) where M : MonadIO<M>, Fallible<M>
    {
        return Optional(User).ToFin(UnAuthorizedError.New("Claims principal of user could not be retrieved!"))
            .Map(p => p.IsInRole(role.Name))
            .Match(b => b ? M.Pure(unit) : M.Fail<Unit>(error), M.Fail<Unit>);
    }

    public K<M, Unit> HasRoleF<M>(Role role, Error error) where M : Monad<M>, Fallible<M>
    {
        return Optional(User).ToFin(UnAuthorizedError.New("Claims principal of user could not be retrieved!"))
            .Map(p => p.IsInRole(role.Name))
            .Match(b => b ? M.Pure(unit) : M.Fail<Unit>(error), M.Fail<Unit>);
    }

    public K<M, Unit> IsSameUser<M>(UserId userId, Error error) where M : MonadIO<M>, Fallible<M>
    {
        return Optional(User).ToFin(UnAuthorizedError.New("Claims principal of user could not be retrieved!"))
            .Map(ParseId)
            .Match(id => id == userId.Value ? M.Pure(unit) : M.Fail<Unit>(error), M.Fail<Unit>);
    }
    public K<M, Unit> IsSameUserF<M>(UserId userId, Error error) where M : Monad<M>, Fallible<M>
    {
        return Optional(User).ToFin(UnAuthorizedError.New("Claims principal of user could not be retrieved!"))
            .Map(ParseId)
            .Match(id => id == userId.Value ? M.Pure(unit) : M.Fail<Unit>(error), M.Fail<Unit>);
    }
    public K<M, Unit> HasPermission<M>(Permission permission, Error error) where M : MonadIO<M>, Fallible<M>
    {
        return Optional(User).ToFin(UnAuthorizedError.New("Claims principal of user could not be retrieved!"))
            .Map(p => p.FindAll(Claims.Permissions).Any(c => c.Value == permission.Name))
            .Match(b => b ? M.Pure(unit) : M.Fail<Unit>(error), M.Fail<Unit>);
    }
    public K<M, Unit> HasPermissionF<M>(Permission permission, Error error) where M : Monad<M>, Fallible<M>
    {
        return Optional(User).ToFin(UnAuthorizedError.New("Claims principal of user could not be retrieved!"))
            .Map(p => p.FindAll(Claims.Permissions).Any(c => c.Value == permission.Name))
            .Match(b => b ? M.Pure(unit) : M.Fail<Unit>(error), M.Fail<Unit>);
    }

    private static Fin<Guid> ParseId(ClaimsPrincipal p)
    {
        var idClaim = p.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Optional(idClaim).ToFin(UnAuthorizedError.New("User id claim is missing"))
            .Bind(id => Guid.TryParse(id, out var guid)
                ? guid
                : FinFail<Guid>(Error.New("Invalid Guid for User id"))
            );
    }

    private static Fin<string> GetName(ClaimsPrincipal p)
    {
        var nameClaim = p.FindFirst(ClaimTypes.Name)?.Value;

        return Optional(nameClaim).ToFin(UnAuthorizedError.New("User name claim is missing"));
    }
    private static Fin<string> GetAvatarUrl(ClaimsPrincipal p)
    {
        var avatarUrlClaim = p.FindFirst(Claims.AvatarUrl)?.Value;
        return Optional(avatarUrlClaim).ToFin(UnAuthorizedError.New("User avatar URL claim is missing"));
    }
    private static Fin<string> GetEmail(ClaimsPrincipal p)
    {
        var emailClaim = p.FindFirst(ClaimTypes.Email)?.Value;
        return Optional(emailClaim).ToFin(UnAuthorizedError.New("User emailService claim is missing"));
    }

    private static Fin<bool> GetHasPendingOrders(ClaimsPrincipal p)
    {
        var hasPendingClaim = p.FindFirst(Claims.HasPendingOrders)?.Value;
        return Optional(hasPendingClaim)
            .Map(value => bool.TryParse(value, out var result) && result)
            .IfNone(false);
    }
}