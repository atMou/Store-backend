namespace Shared.Persistence;

public interface IDataSeeder
{
    Task<Unit> SeedAsync();
    Task<bool> HasDataAsync<TContext>(TContext db) where TContext : DbContext;
}
