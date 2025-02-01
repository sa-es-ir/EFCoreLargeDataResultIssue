using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using Microsoft.Data.SqlClient;

namespace SqlClientProvider;

[MemoryDiagnoser(false)]
[Config(typeof(Config))]
[HideColumns(Column.RatioSD, Column.AllocRatio)]
[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[ShortRunJob]
public class SqlClientBenchmarks
{
    const string ConnectionString = "Server=localhost;Database=EFLargeDataDb;user id=sa;password=P@ssw0rd.123!;TrustServerCertificate=True";

    private class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle =
                SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            IF OBJECT_ID('dbo.TextTable2MB', 'U') IS NULL
                CREATE TABLE TextTable2MB (Id INT IDENTITY(1,1) NOT NULL, Text NVARCHAR(MAX) NOT NULL)
            IF OBJECT_ID('dbo.TextTable5MB', 'U') IS NULL
                CREATE TABLE TextTable5MB (Id INT IDENTITY(1,1) NOT NULL, Text NVARCHAR(MAX) NOT NULL)    
        ";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "DELETE TextTable2MB; DELETE TextTable5MB;";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "INSERT INTO TextTable2MB (Text) VALUES (@text)";
        cmd.Parameters.AddWithValue("text", new string('x', 1024 * 1024 * 2));
        cmd.ExecuteNonQuery();

        cmd.CommandText = "INSERT INTO TextTable5MB (Text) VALUES (@text)";
        cmd.Parameters.AddWithValue("text", new string('x', 1024 * 1024 * 5));
        cmd.ExecuteNonQuery();
    }

    [BenchmarkCategory("2MB"), Benchmark(Baseline = true)]
    public async Task Async2MB()
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand("SELECT top 1 text FROM TextTable2MB", conn);
        await conn.OpenAsync();

        _ = ((string?)await cmd.ExecuteScalarAsync())!.Length;
    }

    [BenchmarkCategory("2MB"), Benchmark]
    public void Sync2MB()
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand("SELECT top 1 text FROM TextTable2MB", conn);
        conn.Open();

        _ = ((string)cmd.ExecuteScalar()).Length;
    }

    [BenchmarkCategory("5MB"), Benchmark(Baseline = true)]
    public async Task Async5MB()
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand("SELECT top 1 text FROM TextTable5MB", conn);
        await conn.OpenAsync();

        _ = ((string?)await cmd.ExecuteScalarAsync())!.Length;
    }

    [BenchmarkCategory("5MB"), Benchmark]
    public void Sync5MB()
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand("SELECT top 1 text FROM TextTable5MB", conn);
        conn.Open();

        _ = ((string)cmd.ExecuteScalar()).Length;
    }
}
