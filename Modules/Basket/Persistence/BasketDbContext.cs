using System.Reflection;

using Basket.Domain.Models;

using Microsoft.EntityFrameworkCore;

namespace Basket.Persistence;

public class BasketDbContext(DbContextOptions<BasketDbContext> options) : DbContext(options)
{
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("basket");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
