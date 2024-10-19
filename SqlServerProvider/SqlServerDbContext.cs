using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SqlServerProvider;

public class SqlServerDbContext : DbContext
{
    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=EFLargeDataDb;User Id=sa;Password=Password123");
    }

    public DbSet<TextTable2MB> TextTable2MB { get; set; } = null!;

    public DbSet<TextTable5MB> TextTable5MB { get; set; } = null!;
}

public class SqlServerDbContextDesignFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
{
    public SqlServerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=EFLargeDataDb;User Id=sa;Password=Password123");

        return new SqlServerDbContext(optionsBuilder.Options);
    }
}