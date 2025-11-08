using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.Persistence;

public static class Extensions
{
    public static Unit UseMigration<TContext>(this IApplicationBuilder app) where TContext : DbContext =>
        Try.lift(async () =>
            {
                using var scope = app.ApplicationServices.CreateScope();
                var db = scope.ServiceProvider.GetService<TContext>();
                var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
                if (db is null)
                {
                    var logger = app.ApplicationServices
                        .GetService<ILoggerFactory>()?
                        .CreateLogger("CatalogModule");
                    logger?.LogWarning("CatalogDbContext service not found during initialization.");
                    return unit;
                }


                var databaseExists = await db.Database.CanConnectAsync();
                if (!databaseExists)
                {
                    await db.Database.MigrateAsync();
                }


                foreach (IDataSeeder seeder in seeders)
                {
                    var hasData = await seeder.HasDataAsync(db);
                    if (!hasData)
                    {
                        await seeder.SeedAsync();
                    }
                }

                return unit;
            })
            .Match(
                _ => unit,
                Fail: ex =>
                {
                    var logger = app.ApplicationServices
                        .GetService<ILoggerFactory>()?
                        .CreateLogger("CatalogModule");
                    logger?.LogError(ex, "An error occurred while migrating or initializing the database.");
                    return unit;
                }
            );
}
