using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Product.Domain.Models;


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

        builder.Property(p => p.Discount).HasColumnName("discount");
        builder.Property(p => p.Price).HasColumnName("price");
        builder.Property(p => p.NewPrice).HasColumnName("new_price");

        builder.Property(p => p.TotalSales).HasColumnName("total_sold_items");
        builder.Property(p => p.TotalReviews).HasColumnName("total_reviews");
        builder.Property(p => p.IsDeleted).HasColumnName("is_Deleted");
        builder.Property(p => p.Brand).HasColumnName("brand");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.CreatedBy).HasColumnName("created_by");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedBy).HasColumnName("updated_by");


        builder.Property(p => p.Brand).HasConversion(bc => bc.Name, s => Brand.FromUnsafe(s));
        builder.Property(p => p.Description).HasConversion(d => d.Value, s => Description.FromUnsafe(s));
        builder.Property(p => p.Price).HasConversion(p => p.Value, d => Money.FromUnSafe(d));
        builder.Property(p => p.Discount).HasConversion(d => d.Value, d => Discount.FromUnsafe(d));

        builder.Property(p => p.NewPrice)
            .HasConversion<decimal?>(
                p => p == null ? null : p.Value,
                d => d == null ? null : Money.Nullable(d)
            );


        builder.OwnsMany(p => p.MaterialDetails, md =>
        {
            md.ToTable("product_material_details");

            // shadow FK to the owning Product
            md.WithOwner().HasForeignKey("product_id");

            md.HasKey("product_id", nameof(MaterialDetail.Detail));
            // scalar properties
            md.Property(m => m.Detail).HasColumnName("detail");
            md.Property(m => m.Percentage).HasColumnName("percentage");

            // map Material value object as an owned object and persist its Name
            md.OwnsOne(m => m.Material, mat =>
            {
                mat.Property(x => x.Name)
                    .HasColumnName("material")
                    .HasColumnType("nvarchar(450)");

                // optional: index on material column
                mat.HasIndex(x => x.Name).HasDatabaseName("IX_ProductMaterialDetails_Material");
            });

            // index on FK for queries
            md.HasIndex("product_id");
        });

        builder.OwnsOne(p => p.Category, c =>
        {
            c.Property(x => x.Main).HasColumnName("category_main");
            c.Property(x => x.Sub).HasColumnName("category_sub");

            c.HasIndex(x => x.Main);
            c.HasIndex(x => x.Sub);
        });
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

        builder.OwnsOne(p => p.ProductType,
            pt =>
            {
                pt.Property(type => type.Type)
                    .HasColumnName("product_type");

                pt.Property(type => type.SubType)
                    .HasColumnName("product_subtype");

                pt.HasIndex(type => type.Type);
                pt.HasIndex(type => type.SubType);
            });

        builder.OwnsMany(p => p.DetailsAttributes,
            attr =>
            {
                attr.ToTable("product_details_attributes");
                attr.WithOwner().HasForeignKey("product_id");

                attr.HasKey("product_id", nameof(Attribute.Name));
                attr.Property(a => a.Name).HasColumnName("attribute_name");

                // convert the Description value to string and back
                attr.Property(a => a.Description)
                    .HasConversion(
                        d => d.Value,
                        s => Description.FromUnsafe(s)
                    )
                    .HasColumnName("attribute_description");
            });

        builder.OwnsMany(p => p.SizeFitAttributes,
            attr =>
            {
                attr.ToTable("product_sizefit_attributes");
                attr.WithOwner().HasForeignKey("product_id");

                attr.HasKey("product_id", nameof(Attribute.Name));
                attr.Property(a => a.Name).HasColumnName("attribute_name");

                attr.Property(a => a.Description)
                    .HasConversion(
                        d => d.Value,
                        s => Description.FromUnsafe(s)
                    )
                    .HasColumnName("attribute_description");
            });

        builder.OwnsOne(p => p.Slug,
            ps =>
            {
                ps.Property(slug => slug.Value)
                    .HasColumnName("slug");

                ps.HasIndex(slug => slug.Value);
            });


        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(p => p.ParentProductId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : ProductId.From(value.Value))
            .HasColumnName("parent_product_id");


        builder.HasMany(p => p.Alternatives)
            .WithOne(p => p.ParentProduct)
            .HasForeignKey(p => p.ParentProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsMany(p => p.Images,
            img =>
            {
                img.ToTable("images");

                img.WithOwner().HasForeignKey(image => image.ProductId);
                img.HasKey(image => image.Id);

                img.Property(i => i.Id)
                    .HasConversion(id => id.Value, v => ImageId.From(v))
                    .ValueGeneratedNever();

                img.OwnsOne(i => i.ImageUrl, nb =>
                {
                    nb.Property(url => url.Value).HasColumnName("image_url");
                    nb.Property(url => url.PublicId).HasColumnName("image_publicId");
                });

                img.Property(i => i.AltText).HasColumnName("alt_text");

                img.Property(i => i.IsMain)
                    .HasColumnName("is_main");
            });

        builder.Property(p => p.AverageRating)
            .HasColumnName("average_rating");
        ;


        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(review => review.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        //builder.HasIndex(p => p.Slug);
        builder.HasIndex(p => p.Brand);
        //builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Price);
        builder.HasIndex(p => p.TotalSales);
        builder.HasIndex(p => p.TotalReviews);
        builder.HasIndex(p => p.AverageRating);
        builder.HasIndex(p => p.Discount);
        builder.ToTable("Products");
    }
}