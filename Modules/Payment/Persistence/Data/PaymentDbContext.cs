using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace Payment.Persistence.Data;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
	public DbSet<Domain.Models.Payment> Payments => Set<Domain.Models.Payment>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{



		modelBuilder.HasDefaultSchema("payments");
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(modelBuilder);
	}
}
