using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Product.Domain.Models;

namespace Product.Persistence;

internal class ColorVariantEntityConfiguration : IEntityTypeConfiguration<ColorVariant>
{
    public void Configure(EntityTypeBuilder<ColorVariant> builder)
    {
        builder.ToTable("variants");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, g => ColorVariantId.From(g))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(v => v.ProductId)
            .HasConversion(id => id.Value, g => ProductId.From(g))
            .IsRequired()
            .HasColumnName("product_id");

        builder.HasOne(v => v.Product)
            .WithMany(p => p.ColorVariants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.Property(v => v.Color)
            .HasConversion(c => c.Name, s => Color.FromUnsafe(s))
            .HasColumnName("color")
            .IsRequired();

        builder.OwnsMany(v => v.SizeVariants, sv =>
        {
            sv.ToTable("size_variants");

            sv.WithOwner().HasForeignKey("variant_id");

            sv.Property(s => s.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            sv.HasKey(s => s.Id);

            sv.Property(s => s.Size)
                .HasConversion(size => size.Name, s => Size.FromUnsafe(s))
                .HasColumnName("size")
                .IsRequired();
            sv.Property(v => v.Sku)
                .HasConversion(sku => sku.Value, s => Sku.FromUnsafe(s))
                .HasColumnName("sku")
                .IsRequired();

            sv.Property(s => s.Stock)
                .HasColumnName("stock")
                .IsRequired();

            sv.Property(s => s.StockLevel)
                .HasConversion<string>()
                .HasColumnName("stock_level")
                .IsRequired();

            sv.HasIndex(s => s.Sku).IsUnique();
            sv.HasIndex("variant_id");
        });

        builder.OwnsMany(v => v.Images, img =>
        {
            img.ToTable("variant_images");

            img.WithOwner().HasForeignKey(image => image.ColorVariantId);

            img.HasKey(image => image.Id);

            img.Property(i => i.Id)
                .HasConversion(id => id.Value, vId => ImageId.From(vId))
                .HasColumnName("id")
                .ValueGeneratedNever();

            img.OwnsOne(i => i.ImageUrl, url =>
            {
                url.Property(u => u.Value).HasColumnName("image_url").IsRequired();
                url.Property(u => u.PublicId).HasColumnName("image_public_id");
            });

            img.Property(i => i.AltText).HasColumnName("alt_text");
            img.Property(i => i.IsMain).HasColumnName("is_main");

            img.HasIndex(i => i.ColorVariantId);
        });

        builder.HasIndex(v => v.ProductId);
        builder.HasIndex(v => v.Color);

        builder.HasIndex(v => new { v.ProductId, v.Color });
    }
}
