namespace Basket.Persistence;

public class BasketDbContext(DbContextOptions<BasketDbContext> options) : DbContext(options)
{
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<LineItem> CartItems => Set<LineItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.AddOutboxMessageEntity();
        //modelBuilder.AddOutboxStateEntity();
        //modelBuilder.AddInboxStateEntity();
        modelBuilder.HasDefaultSchema("basket");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
