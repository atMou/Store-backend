using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence;

internal class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => new { r.UserId, r.Id });

        builder.Property(r => r.Id)
            .HasColumnName("refresh_token_id")
            .IsRequired();


        builder.Property(r => r.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(r => r.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(r => r.RevokedAt).HasColumnName("revoked_at");
        builder.Property(r => r.RevokedReason)
            .HasColumnName("revoked_reason")
            .HasMaxLength(1024);

        builder.Property(r => r.IsRevoked)
            .HasColumnName("is_revoked")
            .IsRequired();

        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, guid => UserId.From(guid))
            .HasColumnName("user_id")
            .IsRequired();


        builder.ToTable("refresh_tokens");
    }
}

