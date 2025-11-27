using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace Inventory.Persistence;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Models.Inventory> Users => Set<Domain.Models.Inventory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.HasDefaultSchema("inventory");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }


}
