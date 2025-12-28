using Product.Domain.Models;
using Shared.Persistence.Extensions;

namespace Product.Persistence;

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
		modelBuilder.Ignore<Sku>();
		modelBuilder.Ignore<ProductId>();
		modelBuilder.Ignore<VariantId>();

		modelBuilder.HasDefaultSchema("products");
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		
		// Apply global filters automatically to all entities implementing ISoftDeletable
		modelBuilder.ApplyGlobalFilters();
		
		// Alternative: Apply specific filter manually
		// modelBuilder.Entity<Domain.Models.Product>()
		//     .HasQueryFilter(p => !p.IsDeleted);
		
		base.OnModelCreating(modelBuilder);
	}
}
