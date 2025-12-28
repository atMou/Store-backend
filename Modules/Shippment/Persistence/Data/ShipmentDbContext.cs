using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Shipment.Persistence.Data;

public class ShipmentDbContext(DbContextOptions<ShipmentDbContext> options) : DbContext(options)
{
	public DbSet<Domain.Models.Shipment> Shipments => Set<Domain.Models.Shipment>();
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

		modelBuilder.HasDefaultSchema("shipments");
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(modelBuilder);
	}
}
