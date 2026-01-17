using Product.Domain.Models;

using Brand = Shared.Domain.ValueObjects.Brand;

namespace Product.Persistence;

public class ProductDBContext(DbContextOptions<ProductDBContext> options, IUserContext userContext) : DbContext(options)
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
        modelBuilder.Ignore<ColorVariantId>();

        //modelBuilder.ApplyGlobalFilterButIgnoreIfHasPermission<Domain.Models.Product>(
        //    [Permission.ManageAllProducts, Permission.ManageInventory],
        //    p => p.HasInventory, userContext);

        //modelBuilder.ApplyGlobalFilterButIgnoreIfHasPermission<Domain.Models.Product>(
        //    [Permission.ManageAllProducts, Permission.ManageInventory],
        //    p => p.IsDeleted, userContext);

        modelBuilder.HasDefaultSchema("products");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        base.OnModelCreating(modelBuilder);
    }
}
