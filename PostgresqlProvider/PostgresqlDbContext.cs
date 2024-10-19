using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PostgresqlProvider;

public class PostgresqlDbContext : DbContext
{
    public PostgresqlDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=EFLargeDataDb;Username=postgres;Password=postgres");
    }

    public DbSet<TextTable2MB> TextTable2MB { get; set; } = null!;

    public DbSet<TextTable5MB> TextTable5MB { get; set; } = null!;
}


public class PostgresqlDbContextDesignFactory : IDesignTimeDbContextFactory<PostgresqlDbContext>
{
    public PostgresqlDbContext CreateDbContext(string[] args)
    {
        return new PostgresqlDbContext();
    }
}