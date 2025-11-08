using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Product.Persistence.Data;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Domain.Models.Product>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Product> builder)
    {

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value)
            )
            .HasColumnName("id");

        builder.Property(p => p.Stock).HasColumnName("stock");
        builder.Property(p => p.LowStockThreshold).HasColumnName("low_stock_threshold");
        builder.Property(p => p.TotalReviews).HasColumnName("total_reviews");
        builder.Property(p => p.TotalSales).HasColumnName("total_sold_items");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.CreatedBy).HasColumnName("created_by");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedBy).HasColumnName("updated_by");
        builder.Property("_averageRating").HasColumnName("average_rating");


        builder.HasMany(p => p.Variants).WithOne()
            .HasForeignKey("product_id").OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(p => p.Slug,
            sl =>
                sl.Property(s => s.Value)
                    .HasColumnName("slug")
                    .IsRequired());

        builder.Property(p => p.Price).HasConversion(
                price => price.Value,
                value => Money.FromDecimal(value))
            .IsRequired().HasColumnName("price")
            .HasPrecision(8, 2);


        builder.Property<Money>("_newPrice")
            .HasConversion(m => m.Value, v => Money.FromDecimal(v))
            .HasPrecision(8, 2)
            .HasColumnName("new_price");


        builder.OwnsOne(p => p.ProductStatus,
            ps =>
            {
                ps.Property(status => status.IsTrending)
                    .HasColumnName("is_trending");
                ps.Property(status => status.IsBestSeller)
                    .HasColumnName("is_best_seller");
                ps.Property(status => status.IsFeatured)
                    .HasColumnName("is_featured");
                ps.Property(status => status.IsNew)
                    .HasColumnName("is_new");
            });

        builder.OwnsOne(p => p.Sku, sk =>
            sk.Property(s => s.Value)
                .HasColumnName("sku")
                .IsRequired());


        builder.Property(p => p.Size)
            .HasConversion(s => s.Code.ToString(), v => Size.FromUnsafe(v))
            .HasColumnName("size");


        builder.OwnsMany(p => p.ImageUrls, iu =>
        {
            iu.WithOwner().HasForeignKey("product_id");
            iu.Property(u => u.Value)
                .IsRequired()
                .HasColumnName("image_url");

            iu.ToTable("product_image_urls");
        });


        builder.Property(p => p.Currency)
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromUnsafe(code)
            )
            .HasColumnName("currency")
            .IsRequired();


        builder.Property(p => p.Brand)
            .HasConversion(bc => bc.Code.ToString(),
                s => Brand.FromUnsafe(s)).HasColumnName("brand")
            .IsRequired();

        builder.Property(p => p.Color)
            .HasConversion(cc => cc.Code.ToString(),
                s => Color.FromUnsafe(s)).HasColumnName("color")
            .IsRequired();

        builder.Property(p => p.Category)
            .HasConversion(cc => cc.Code.ToString(),
                s => Category.FromUnsafe(s)).HasColumnName("category")
            .IsRequired();

        builder.OwnsOne(p => p.Description, d =>
            d.Property(desc => desc.Value)
                .HasColumnName("description").IsRequired());
        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey("product_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);


        builder.ToTable("Products");
    }
}