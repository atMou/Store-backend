using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configurations;

internal class LikedProductsIdsEntityConfiguration : IEntityTypeConfiguration<LikedProductId>
{
	public void Configure(EntityTypeBuilder<LikedProductId> builder)
	{
		builder.HasKey(lp => new { lp.UserId, lp.ProductId });
		builder.Property(lp => lp.UserId).HasConversion(id => id.Value, guid => UserId.From(guid))
			.IsRequired();
		builder.Property(lp => lp.ProductId).HasConversion(id => id.Value, guid => ProductId.From(guid))
			.IsRequired();

		builder.ToTable("liked_products_ids");
	}
}

