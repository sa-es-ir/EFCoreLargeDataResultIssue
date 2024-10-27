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
public class Benchmarks
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

    //[GlobalSetup]
    //public void Setup()
    //{
    //    using var conn = new SqlConnection(ConnectionString);
    //    conn.Open();

    //    using var cmd = conn.CreateCommand();
    //    cmd.CommandText = @"
    //IF OBJECT_ID('dbo.data', 'U') IS NOT NULL
    //DROP TABLE data; 
    //CREATE TABLE data (id INT, foo VARBINARY(MAX))
    //";
    //    cmd.ExecuteNonQuery();

    //    cmd.CommandText = "INSERT INTO data (id, foo) VALUES (@id, @foo)";
    //    cmd.Parameters.AddWithValue("id", 1);
    //    cmd.Parameters.AddWithValue("foo", new byte[1024 * 1024 * 10]);
    //    cmd.ExecuteNonQuery();
    //}

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
