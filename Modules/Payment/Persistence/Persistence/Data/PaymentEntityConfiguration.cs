using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Order.Domain.ValueObjects;

using Payment.Domain.ValueObjects;

namespace Payment.Persistence.Persistence.Data;

public class PaymentEntityConfiguration : IEntityTypeConfiguration<Domain.Models.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => PaymentId.From(value)
            )
            .HasColumnName("id");
        builder.Property(p => p.OrderId).HasConversion(id => id.Value, guid => OrderId.From(guid));
        builder.Property(p => p.PaymentStatus)
            .HasConversion(status => status.Name, s => PaymentStatus.FromUnsafe(s))
            .HasColumnName("status");

        builder.Property(p => p.PaymentMethod)
            .HasConversion(method => method.Name, s => PaymentMethod.FromUnsafe(s));

        builder.Property("_transactionId").HasColumnName("transaction_id");
        builder.Property("_refundedAt").HasColumnName("refunded_at");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.CreatedBy).HasColumnName("created_by");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedBy).HasColumnName("updated_by");



        builder.HasIndex(payment => payment.OrderId);
        builder.ToTable("payments");



    }
}
