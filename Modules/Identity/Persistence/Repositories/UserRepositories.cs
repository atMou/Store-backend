using Db.Errors;

using Identity.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;

using Shared.Domain.Errors;

namespace Identity.Persistence.Repositories;

public class UserRepositories : IUserRepository
{
    public IO<User> GetUserById(UserId userId, IdentityDbContext ctx)
    {
        return from u in IO.liftAsync<User?>(async e => await ctx.Users.FindAsync([userId], e.Token))
               from _ in when(u is null,
                   IO.fail<Unit>(NotFoundError.New($"User with id: '{userId.Value}' does not exists")))
               select u;
    }

    public IO<Unit> AddUser(User user, IdentityDbContext ctx)
    {
        return IO.lift(_ =>
        {
            ctx.Users.Add(user);
            return Unit.Default;
        });
    }

    public IO<User> GetUserByEmail(Email email, IdentityDbContext ctx)
    {
        return from u in IO.liftAsync<User?>(async e =>
                await ctx.Users.FirstOrDefaultAsync(u => u.Email == email, e.Token))
               from _ in when(u is null,
                   IO.fail<Unit>(NotFoundError.New($"User with email: '{email.Value}' does not exists")))
               select u;
    }

    public IO<Unit> CheckIfUserExists(Email email, IdentityDbContext ctx)
    {
        return from u in IO.liftAsync<User?>(async e =>
                await ctx.Users.FirstOrDefaultAsync(u => u.Email == email, e.Token))
               from _ in when(u is not null,
                   IO.fail<Unit>(ConflictError.New($"User with email: '{email.Value}' already exists")))
               select unit;
    }
}