using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;

namespace SqlClientProvider;

[MemoryDiagnoser]
[ShortRunJob]
public class Benchmarks
{
    const string ConnectionString = "Server=localhost;Database=EFLargeDataDb;user id=sa;password=P@ssw0rd.123!;TrustServerCertificate=True";


    //    [GlobalSetup]
    //    public void Setup()
    //    {
    //        using var conn = new SqlConnection(ConnectionString);
    //        conn.Open();

    //        using var cmd = conn.CreateCommand();
    //        cmd.CommandText = @"
    //IF OBJECT_ID('dbo.data', 'U') IS NOT NULL
    //DROP TABLE data; 
    //CREATE TABLE data (id INT, foo VARBINARY(MAX))
    //";
    //        cmd.ExecuteNonQuery();

    //        cmd.CommandText = "INSERT INTO data (id, foo) VALUES (@id, @foo)";
    //        cmd.Parameters.AddWithValue("id", 1);
    //        cmd.Parameters.AddWithValue("foo", new byte[1024 * 1024 * 10]);
    //        cmd.ExecuteNonQuery();
    //    }

    [Benchmark(Baseline = true)]
    public async Task<int> Async()
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand("SELECT top 1 text FROM TextTable5MB", conn);
        await conn.OpenAsync();

        return ((string)await cmd.ExecuteScalarAsync()).Length;
    }

    [Benchmark]
    public void Sync()
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand("SELECT top 1 text FROM TextTable5MB", conn);
        conn.Open();

        _ = ((string)cmd.ExecuteScalar()).Length;
    }

}
