using Product.Domain.Models;

namespace Product.Persistence.Data;

public class ProductDBContext(DbContextOptions<ProductDBContext> options) : DbContext(options)
{
    public DbSet<Domain.Models.Product> Products => Set<Domain.Models.Product>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.AddOutboxMessageEntity();
        //modelBuilder.AddOutboxStateEntity();
        //modelBuilder.AddInboxStateEntity();
        modelBuilder.Ignore<Size>();
        modelBuilder.Ignore<Brand>();
        modelBuilder.Ignore<Color>();
        modelBuilder.Ignore<Category>();
        modelBuilder.HasDefaultSchema("products");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
