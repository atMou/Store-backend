using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Order.Domain.ValueObjects;

using Payment.Domain.ValueObjects;

namespace Payment.Persistence.Data;

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
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(p => p.OrderId)
            .HasConversion(id => id.Value, guid => OrderId.From(guid))
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(p => p.UserId)
            .HasConversion(id => id.Value, guid => UserId.From(guid))
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(p => p.CartId)
            .HasConversion(id => id.Value, guid => CartId.From(guid))
            .HasColumnName("cart_id")
            .IsRequired();

        builder.Property(p => p.Tax)
            .HasColumnName("tax")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Total)
            .HasColumnName("total")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.PaymentStatus)
            .HasConversion(status => status.Name, s => PaymentStatus.FromUnsafe(s))
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.PaymentMethod)
            .HasConversion(method => method.Name, s => PaymentMethod.FromUnsafe(s))
            .HasColumnName("payment_method")
            .HasMaxLength(50)
            .IsRequired();

        // TransactionId is non-nullable string in domain model
        builder.Property(p => p.TransactionId)
            .HasColumnName("transaction_id")
            .HasMaxLength(255)
            .IsRequired(false); // but can be null in database until payment is fulfilled

        builder.Property(p => p.RefundId)
            .HasColumnName("refund_id")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(p => p.RefundAmount)
            .HasColumnName("refund_amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        // RefundStatus is nullable string in domain model (marked with [NotMapped] but has init)
        builder.Property(p => p.RefundStatus)
            .HasColumnName("refund_status")
            .HasMaxLength(50)
            .IsRequired(false);

        // PaidAt, RefundedAt, FailedAt are non-nullable DateTime in domain model
        // but default to DateTime.MinValue until set
        builder.Property(p => p.PaidAt)
            .HasColumnName("paid_at")
            .IsRequired(false); // nullable in DB to handle default values

        builder.Property(p => p.RefundedAt)
            .HasColumnName("refunded_at")
            .IsRequired(false);

        builder.Property(p => p.FailedAt)
            .HasColumnName("failed_at")
            .IsRequired(false);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(p => p.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(255)
            .IsRequired(false);

        // Explicitly ignore [NotMapped] properties
        builder.Ignore(p => p.Currency);
        builder.Ignore(p => p.ClientSecret);
        builder.Ignore(p => p.Amount);

        // Indexes for performance
        builder.HasIndex(p => p.OrderId);
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.CartId);
        builder.HasIndex(p => p.TransactionId);
        builder.HasIndex(p => p.PaymentStatus);
        builder.HasIndex(p => p.CreatedAt);

        builder.ToTable("payments");
    }
}
