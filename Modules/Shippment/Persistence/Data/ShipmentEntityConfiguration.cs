using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipment.Domain.ValueObjects;

namespace Shipment.Persistence.Data;

public class ShipmentEntityConfiguration : IEntityTypeConfiguration<Shipment.Domain.Models.Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment.Domain.Models.Shipment> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, guid => ShipmentId.From(guid))
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.OrderId)
            .HasConversion(id => id.Value, guid => OrderId.From(guid))
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(s => s.ShippingStatus)
            .HasConversion(status => status.Name, name => ShippingStatus.From(name))
            .HasColumnName("shipping_status")
            .IsRequired();

        builder.OwnsOne(s => s.ShippingAddress, nb =>
        {
            nb.Property(address => address.City).HasColumnName("city")
                .IsRequired();

            nb.Property(address => address.Street).HasColumnName("street")
                .IsRequired();

            nb.Property(address => address.HouseNumber).HasColumnName("house_number")
                .IsRequired();

            nb.Property(address => address.PostalCode).HasColumnName("postal_code")
                .IsRequired();

            nb.Property(address => address.ExtraDetails).HasColumnName("extra_details");
        });

        builder.Property("_ShippedAt")
            .HasColumnName("shipped_at");

        builder.Property("_DeliveredAt")
            .HasColumnName("delivered_at");

        builder.Property(s => s.TrackingCode)
            .HasColumnName("tracking_code")
            .IsRequired();

        builder.Ignore(s => s.ShippedAt);
        builder.Ignore(s => s.DeliveredAt);
    }
}
