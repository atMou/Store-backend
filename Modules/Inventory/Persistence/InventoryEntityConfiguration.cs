using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Persistence;
internal class InventoryEntityConfiguration : IEntityTypeConfiguration<Domain.Models.Inventory>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Inventory> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .HasColumnName("inventory_id");

        builder.Property(i => i.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .HasColumnName("product_id");

        builder.OwnsOne(i => i.Stock, s =>

        {
            s.Property(st => st.Value).HasColumnName("stock_value");
            s.Property(st => st.Low).HasColumnName("stock_low");
            s.Property(st => st.Mid).HasColumnName("stock_mid");
            s.Property(st => st.High).HasColumnName("stock_high");
        });



        builder.Property(i => i.Reserved)
            .HasColumnName("reserved");

        builder.Property(i => i.Version).IsRowVersion()
            .HasColumnName("version");

        builder.Ignore(i => i.StockLevel);

        builder.ToTable("inventories");


    }
}
