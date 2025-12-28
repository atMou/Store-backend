namespace Identity.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
	public DbSet<User> Users => Set<User>();
	public DbSet<LikedProductId> LikedProductsIds => Set<LikedProductId>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//modelBuilder.AddOutboxMessageEntity();
		//modelBuilder.AddOutboxStateEntity();
		//modelBuilder.AddInboxStateEntity();
		modelBuilder.Ignore<OrderId>();
		modelBuilder.HasDefaultSchema("identity");
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(modelBuilder);
	}


}
