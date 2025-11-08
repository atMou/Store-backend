using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Shared.Infrastructure.ValueObjects;

namespace Shared.Infrastructure.Authentication;

public record CurrentUser(Guid Id, string Email, string Name)
{
}

public interface IUserContext
{
    K<M, CurrentUser> GetCurrentUser<M>() where M : MonadIO<M>, Fallible<M>;
    K<M, bool> IsInRole<M>(Role role) where M : MonadIO<M>, Fallible<M>;
    K<M, bool> IsSameUser<M>(UserId userId, Error error) where M : MonadIO<M>, Fallible<M>;
    K<M, bool> HasPermission<M>(Permission permission, Error error) where M : MonadIO<M>, Fallible<M>;
}

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public K<M, CurrentUser> GetCurrentUser<M>() where M : MonadIO<M>, Fallible<M>
    {

        Fin<CurrentUser> currentUser = CheckClaimsPrincipalIsNotNull(User)
            .Bind(p => (ParseId(p), GetEmail(p), GetName(p))
                .Apply((id, email, name) => new CurrentUser(id, email, name)));

        return M.LiftIO(currentUser.Match(u => IO.lift(() => u), IO.fail<CurrentUser>));
    }

    private static Fin<ClaimsPrincipal> CheckClaimsPrincipalIsNotNull(ClaimsPrincipal? p)
    {
        return Optional(p).ToFin(Error.New("Claims principal of user could not be retrieved!"));
    }

    private static Fin<Guid> ParseId(ClaimsPrincipal p)
    {
        return Optional(p.FindFirst(ClaimTypes.NameIdentifier)?.Value).ToFin(Error.New("User id claim is missing"))
            .Bind(s => Guid.TryParse(s, out var guid) ? guid : FinFail<Guid>(Error.New("Invalid Guid for User id")));
    }

    private static Fin<string> GetName(ClaimsPrincipal p)
    {
        return Optional(p.FindFirst(ClaimTypes.Name)?.Value).ToFin(Error.New("User name claim is missing"));
    }

    private static Fin<string> GetEmail(ClaimsPrincipal p)
    {
        return Optional(p.FindFirst(ClaimTypes.Email)?.Value).ToFin(Error.New("User email claim is missing"));
    }

    public K<M, bool> IsInRole<M>(Role role) where M : MonadIO<M>, Fallible<M>
    {
        return Optional(User).ToFin(Error.New("Claims principal of user could not be retrieved!"))
            .Map(p => p.IsInRole(role.Name))
            .Match(b => M.LiftIO(IO.lift(() => b)), e => M.LiftIO(IO.fail<bool>(e)));

    }

    public K<M, bool> IsSameUser<M>(UserId userId, Error error) where M : MonadIO<M>, Fallible<M>
    {
        return Optional(User).ToFin(Error.New("Claims principal of user could not be retrieved!"))
            .Map(ParseId)
            .Match(id => id == userId.Value ? M.Pure(true) : M.Fail<bool>(error), e => M.LiftIO(IO.fail<bool>(e)));
    }

    public K<M, bool> HasPermission<M>(Permission permission, Error error) where M : MonadIO<M>, Fallible<M>
    {
        return Optional(User).ToFin(Error.New("Claims principal of user could not be retrieved!"))
            .Map(p => p.FindAll(Claims.Permissions).Any(c => c.Value == permission.Name))
            .Match(b => b ? M.LiftIO(IO.lift(() => b)) : M.Fail<bool>(error), M.Fail<bool>);
    }
}

