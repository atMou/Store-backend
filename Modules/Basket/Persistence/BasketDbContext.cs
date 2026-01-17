using Shared.Persistence.Extensions;

namespace Basket.Persistence;

public class BasketDbContext(DbContextOptions<BasketDbContext> options, IUserContext userContext) : DbContext(options)
{
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<LineItem> CartItems => Set<LineItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyGlobalFilterButIgnoreIfHasPermission<Coupon>(
            [Shared.Domain.ValueObjects.Permission.ManageCoupons],
            coupon => !coupon.IsDeleted, userContext);

        // 
        modelBuilder.ApplyGlobalFilterButIgnoreIfHasPermission<Cart>(
            [Shared.Domain.ValueObjects.Permission.ManageCarts],
            cart => cart.IsActive, userContext);

        //modelBuilder.ApplyGlobalFilterButIgnoreIfHasPermission<Cart>(
        //    [Shared.Domain.ValueObjects.Permission.ManageCarts],
        //    cart => !cart.IsCheckedOut, userContext);

        modelBuilder.Ignore<Description>();
        modelBuilder.HasDefaultSchema("basket");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
