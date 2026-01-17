using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace Inventory.Persistence;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Models.Inventory> Inventory => Set<Domain.Models.Inventory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ColorVariantId>();
        modelBuilder.Ignore<Size>();
        modelBuilder.Ignore<ProductId>();
        modelBuilder.Ignore<InventoryId>();
        modelBuilder.Ignore<Warehouse>();
        modelBuilder.Ignore<Stock>();

        modelBuilder.HasDefaultSchema("inventory");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
