using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SqlServerProvider;

public class SqlServerDbContext : DbContext
{
    private string _connectionString;
    public SqlServerDbContext(string? connectionString = null)
    {
        _connectionString = connectionString
            ?? "Server=localhost;Database=EFLargeDataDb;user id=sa;password=P@ssw0rd.123!;TrustServerCertificate=True";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    public DbSet<TextTable2MB> TextTable2MB { get; set; } = null!;

    public DbSet<TextTable5MB> TextTable5MB { get; set; } = null!;
}

public class SqlServerDbContextDesignFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
{
    public SqlServerDbContext CreateDbContext(string[] args)
    {
        return new SqlServerDbContext();
    }
}