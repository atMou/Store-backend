namespace Identity.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserLikedProduct> UserLikedProducts => Set<UserLikedProduct>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Ignore<OrderId>();
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }


}
