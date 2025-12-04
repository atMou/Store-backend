using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Product.Persistence;

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

        builder.Property(p => p.Slug).HasColumnName("slug");
        builder.Property(p => p.Brand).HasColumnName("brand");
        builder.Property(p => p.Brand).HasColumnName("brand");
        builder.Property(p => p.Category).HasColumnName("category");
        builder.Property(p => p.Discount).HasColumnName("discount");
        builder.Property(p => p.Price).HasColumnName("price");
        builder.Property(p => p.NewPrice).HasColumnName("new_price");

        builder.Property(p => p.TotalSales).HasColumnName("total_sold_items");
        builder.Property(p => p.TotalReviews).HasColumnName("total_reviews");
        builder.Property(p => p.IsDeleted).HasColumnName("is_Deleted");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.CreatedBy).HasColumnName("created_by");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedBy).HasColumnName("updated_by");


        builder.Property(p => p.Slug).HasConversion(slug => slug.Value, s => Slug.FromUnsafe(s));
        builder.Property(p => p.Brand).HasConversion(bc => bc.Code.ToString(), s => Brand.FromUnsafe(s));
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

                ps.HasIndex(status => status.IsNew);
                ps.HasIndex(status => status.IsFeatured);
                ps.HasIndex(status => status.IsBestSeller);
                ps.HasIndex(status => status.IsTrending);
            });





        builder.OwnsMany(p => p.Variants, v =>
        {
            v.ToTable("Variants");
            v.WithOwner().HasForeignKey(variant => variant.ProductId);
            v.HasKey(variant => variant.Id);
            v.Property(variant => variant.Id)
                .HasConversion(id => id.Value, g => VariantId.From(g))
                .ValueGeneratedNever();

            v.Property(variant => variant.Color).HasConversion(color => color.Name, s => Color.FromUnsafe(s));
            v.Property(variant => variant.Size).HasConversion(size => size.Name, s => Size.FromUnsafe(s));
            v.OwnsOne(variant => variant.Stock, stock =>
            {
                stock.Property(s => s.Value).HasColumnName("stock_quantity");
                stock.Property(s => s.Low).HasColumnName("stock_low");
                stock.Property(s => s.Mid).HasColumnName("stock_mid");
                stock.Property(s => s.High).HasColumnName("stock_high");
            });
            v.OwnsMany(variant => variant.Images, img =>
            {
                img.ToTable("VariantImages");
                img.WithOwner().HasForeignKey(image => image.VariantId);
                img.HasKey(image => image.Id);
                img.Property(i => i.Id)
                    .HasConversion(id => id.Value, vId => ProductImageId.From(vId))
                    .ValueGeneratedNever();
                img.Property(i => i.ImageUrl)
                    .HasConversion(url => url.Value, s => ImageUrl.FromUnsafe(s)).HasColumnName("image_url");
                img.Property(i => i.AltText).HasColumnName("alt_text");
                img.Property(i => i.IsMain)
                    .HasColumnName("is_main");
            });
        });

        builder.HasMany(p => p.Alternatives)
            .WithOne(p => p.ParentProduct)
            .HasForeignKey(p => p.ParentProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsMany(p => p.Images, img =>
        {
            img.ToTable("ImageDtos");

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

        builder.Property(p => p.AverageRating)
            .HasColumnName("average_rating"); ;



        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(review => review.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(p => p.Slug);
        builder.HasIndex(p => p.Brand);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Price);
        builder.HasIndex(p => p.TotalSales);
        builder.HasIndex(p => p.TotalReviews);
        builder.HasIndex(p => p.AverageRating);
        builder.HasIndex(p => p.Discount);
        builder.ToTable("Products");
    }
}
