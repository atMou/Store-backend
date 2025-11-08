using Microsoft.EntityFrameworkCore;

using Shared.Persistence;

namespace Basket.Persistence.Seeder;

public class BasketDataSeeder : IDataSeeder
{
    public Task SeedAsync(BasketDbContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        // Seed initial data for the basket module
        return Task.CompletedTask;
    }

    public Task<Unit> SeedAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasDataAsync<TContext>(TContext db) where TContext : DbContext
    {
        throw new NotImplementedException();
    }
}
