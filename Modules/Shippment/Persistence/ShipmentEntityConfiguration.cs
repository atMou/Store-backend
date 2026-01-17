using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipment.Domain.ValueObjects;

namespace Shipment.Persistence;

public class ShipmentEntityConfiguration : IEntityTypeConfiguration<Domain.Models.Shipment>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Shipment> builder)
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
            nb.Property(address => address.ReceiverName).HasColumnName("receiver_name")
                .HasMaxLength(200)
                .IsRequired();

            nb.Property(address => address.City).HasColumnName("city")
                .HasMaxLength(100)
                .IsRequired();

            nb.Property(address => address.Street).HasColumnName("street")
                .HasMaxLength(200)
                .IsRequired();

            nb.Property(address => address.HouseNumber).HasColumnName("house_number")
                .IsRequired();

            nb.Property(address => address.PostalCode).HasColumnName("postal_code")
                .IsRequired();

            nb.Property(address => address.ExtraDetails).HasColumnName("extra_details")
                .HasMaxLength(500);
        });

        builder.Property(s => s.ShippedAt)
            .HasColumnName("shipped_at");

        builder.Property(s => s.DeliveredAt)
            .HasColumnName("delivered_at");

        builder.Property(s => s.TrackingCode)
            .HasColumnName("tracking_code")
            .IsRequired();


    }
}
