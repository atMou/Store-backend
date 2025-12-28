using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Product.Domain.Models;

namespace Product.Persistence;

internal class VariantEntityConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.ToTable("Variants");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, g => VariantId.From(g))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(v => v.ProductId)
            .HasConversion(id => id.Value, g => ProductId.From(g))
            .IsRequired()
            .HasColumnName("product_id");

        builder.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(v => v.Sku)
            .HasConversion(sku => sku.Value, s => Sku.FromUnsafe(s))
            .HasColumnName("sku")
            .IsRequired();

        builder.Property(v => v.Color)
            .HasConversion(c => c.Name, s => Color.FromUnsafe(s))
            .HasColumnName("color")
            .IsRequired();



        builder.OwnsMany(v => v.Images, img =>
        {
            img.ToTable("VariantImages");

            img.WithOwner().HasForeignKey(image => image.VariantId);

            img.HasKey(image => image.Id);

            img.Property(i => i.Id)
                .HasConversion(id => id.Value, vId => ImageId.From(vId))
                .HasColumnName("id")
                .ValueGeneratedNever();

            img.OwnsOne(i => i.ImageUrl, url =>
            {
                url.Property(u => u.Value).HasColumnName("image_url").IsRequired();
                url.Property(u => u.PublicId).HasColumnName("image_publicId");
            });

            img.Property(i => i.AltText).HasColumnName("alt_text");
            img.Property(i => i.IsMain).HasColumnName("is_main");

            img.HasIndex(i => i.VariantId);
        });

        builder.HasIndex(v => v.ProductId);
        builder.HasIndex(v => v.Sku).IsUnique();
        builder.HasIndex(v => v.Color);

        builder.HasIndex(v => new { v.ProductId, v.Color });
    }
}
