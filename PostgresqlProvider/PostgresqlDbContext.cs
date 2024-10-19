using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PostgresqlProvider;

public class PostgresqlDbContext : DbContext
{
    public PostgresqlDbContext(DbContextOptions<PostgresqlDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=EFLargeDataDb;Username=postgres;Password=postgres");
    }

    public DbSet<TextTable2MB> TextTable2MB { get; set; } = null!;

    public DbSet<TextTable5MB> TextTable5MB { get; set; } = null!;
}


public class PostgresqlDbContextDesignFactory : IDesignTimeDbContextFactory<PostgresqlDbContext>
{
    public PostgresqlDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PostgresqlDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=EFLargeDataDb;Username=postgres;Password=postgres");

        return new PostgresqlDbContext(optionsBuilder.Options);
    }
}