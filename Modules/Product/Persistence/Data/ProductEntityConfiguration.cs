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

        builder.Property(p => p.Sku).HasColumnName("sku");
        builder.Property(p => p.Slug).HasColumnName("slug");
        builder.Property(p => p.Brand).HasColumnName("brand");
        builder.Property(p => p.Size).HasColumnName("size");
        builder.Property(p => p.Color).HasColumnName("color");
        builder.Property(p => p.Brand).HasColumnName("brand");
        builder.Property(p => p.Category).HasColumnName("category");
        builder.Property(p => p.Discount).HasColumnName("discount");
        builder.Property(p => p.Price).HasColumnName("price");
        builder.Property(p => p.NewPrice).HasColumnName("new_price");
        builder.Property(p => p.TotalReviews).HasColumnName("total_reviews");
        builder.Property(p => p.TotalSales).HasColumnName("total_sold_items");
        builder.Property("_averageRating").HasColumnName("average_rating");
        builder.Property(p => p.IsDeleted).HasColumnName("is_Deleted");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.CreatedBy).HasColumnName("created_by");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedBy).HasColumnName("updated_by");


        builder.Property(p => p.Slug).HasConversion(slug => slug.Value, s => Slug.FromUnsafe(s));
        builder.Property(p => p.Sku).HasConversion(sku => sku.Value, s => Sku.FromUnsafe(s));
        builder.Property(p => p.Brand).HasConversion(bc => bc.Code.ToString(), s => Brand.FromUnsafe(s));
        builder.Property(p => p.Size).HasConversion(s => s.Code.ToString(), v => Size.FromUnsafe(v));
        builder.Property(p => p.Color).HasConversion(cc => cc.Code.ToString(), s => Color.FromUnsafe(s));
        builder.Property(p => p.Category).HasConversion(cc => cc.Code.ToString(), s => Category.FromUnsafe(s));
        builder.Property(p => p.Description).HasConversion(d => d.Value, s => Description.FromUnsafe(s));
        builder.Property(p => p.Price).HasConversion(p => p.Value, d => Money.FromUnSafe(d));
        builder.Property(p => p.Discount).HasConversion(d => d.Value, d => Discount.FromUnsafe(d));

        builder.Property(p => p.NewPrice)
            .HasConversion<decimal?>(
                p => p == null ? null : p.Value,
                d => d == null ? null : Money.Nullable(d)
            );

        builder.OwnsOne(p => p.Status,
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

        builder.OwnsOne(p => p.Stock,
            ps =>
            {
                ps.Property(stock => stock.Value)
                    .HasColumnName("stock");
                ps.Property(stock => stock.Low)
                    .HasColumnName("low_stock_threshold");
                ps.Property(stock => stock.High)
                    .HasColumnName("high_stock_threshold");
                ps.Property(stock => stock.Mid)
                    .HasColumnName("mid_stock_threshold");
            });

        builder.HasMany(p => p.Variants)
            .WithOne(p => p.ParentProduct)
            .HasForeignKey(p => p.ParentProductId).OnDelete(DeleteBehavior.NoAction);

        builder.OwnsMany(p => p.ProductImages, img =>
        {
            img.ToTable("ProductImages");

            img.WithOwner().HasForeignKey(image => image.ProductId);
            img.HasKey(image => image.Id);

            img.Property(i => i.Id)
                .HasConversion(id => id.Value, v => ProductImageId.From(v))
                .ValueGeneratedNever();

            img.Property(i => i.ImageUrl)
                .HasConversion(url => url.Value, s => ImageUrl.FromUnsafe(s)).HasColumnName("image_url");

            img.Property(i => i.AltText).HasColumnName("alt_text");

            img.Property(i => i.IsMain)
                .HasColumnName("is_main");
        });

        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(review => review.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.ToTable("Products");
    }
}