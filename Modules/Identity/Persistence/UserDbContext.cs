using Identity.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("user");
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
        base.OnConfiguring(optionsBuilder);
    }
}
