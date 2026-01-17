using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shared.Domain.Enums;

namespace Inventory.Persistence;

internal class InventoryEntityConfiguration : IEntityTypeConfiguration<Domain.Models.Inventory>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Inventory> builder)
    {
        builder.ToTable("inventories");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => InventoryId.From(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(i => i.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .IsRequired()
            .HasColumnName("product_id");

        builder.Property(i => i.Brand)
            .HasColumnName("brand")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Slug)
            .HasColumnName("slug")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.ImageUrl)
            .HasColumnName("image_url")
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(i => i.Version)
            .IsRowVersion()
            .HasColumnName("version");

        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(i => i.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(200);

        builder.Property(i => i.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(200);

        builder.OwnsMany(i => i.ColorVariants, cv =>
        {
            cv.ToTable("inventory_color_variants");

            cv.WithOwner().HasForeignKey("inventory_id");

            cv.Property<Guid>("Id")
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            cv.HasKey("Id");

            cv.Property(c => c.ColorVariantId)
                .HasConversion(
                    id => id.Value,
                    value => ColorVariantId.From(value))
                .IsRequired()
                .HasColumnName("color_variant_id");

            cv.Property(c => c.Color)
                .HasColumnName("color")
                .IsRequired()
                .HasMaxLength(100);

            cv.OwnsMany(c => c.SizeVariants, sv =>
            {
                sv.ToTable("inventory_size_variants");

                sv.WithOwner().HasForeignKey("color_variant_id");

                sv.HasKey(s => s.Id);

                sv.Property(s => s.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                sv.Property(s => s.Size)
                    .HasConversion(
                        size => size.Name,
                        name => Size.FromUnsafe(name))
                    .HasColumnName("size")
                    .IsRequired()
                    .HasMaxLength(50);

                sv.OwnsOne(s => s.Stock, stock =>
                {
                    stock.Property(st => st.Value)
                        .HasColumnName("stock_value")
                        .IsRequired();

                    stock.Property(st => st.Low)
                        .HasColumnName("low_threshold")
                        .IsRequired();

                    stock.Property(st => st.Mid)
                        .HasColumnName("mid_threshold")
                        .IsRequired();

                    stock.Property(st => st.High)
                        .HasColumnName("high_threshold")
                        .IsRequired();
                });

                sv.Property(s => s.Reserved)
                    .HasColumnName("reserved")
                    .IsRequired()
                    .HasDefaultValue(0);

                // Store Warehouses as comma-separated string of warehouse codes
                sv.Property(s => s.Warehouses)
                    .HasConversion(
                        warehouses => string.Join(",", warehouses.Select(w => w.Code.ToString())),
                        warehouseCodes => warehouseCodes
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(code => Warehouse.FromUnsafe(code.Trim()))
                            .ToList())
                    .HasColumnName("warehouse_codes")
                    .HasMaxLength(500)
                    .IsRequired();

                sv.Ignore(s => s.AvailableStock);
                sv.Ignore(s => s.StockLevel);

                sv.HasIndex("color_variant_id");
                sv.HasIndex(s => s.Size);
            });

            cv.HasIndex("inventory_id");
            cv.HasIndex(c => c.ColorVariantId);
        });

        builder.Ignore(i => i.TotalAvailableStock);
        builder.Ignore(i => i.TotalStock);
        builder.Ignore(i => i.TotalReserved);
        builder.Ignore(i => i.DomainEvents);

        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.Brand);
        builder.HasIndex(i => i.Slug);
    }
}
