using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace Order.Persistence.Data;

public class OrderDBContext(DbContextOptions<OrderDBContext> options) : DbContext(options)
{
    public DbSet<Domain.Models.Order> Orders { get; set; }
    public DbSet<Domain.Models.OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.HasDefaultSchema("order");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
