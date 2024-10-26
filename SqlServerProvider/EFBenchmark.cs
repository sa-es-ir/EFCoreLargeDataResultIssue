using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace SqlServerProvider;

[MemoryDiagnoser(false)]
[Config(typeof(Config))]
[HideColumns(Column.RatioSD, Column.AllocRatio)]
[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[ShortRunJob]
public class EFSqlServerBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle =
                SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
        }
    }

    [GlobalSetup]
    public async Task Setup()
    {
        var dbcontext = new SqlServerDbContext();

        await dbcontext.TextTable2MB.ExecuteDeleteAsync();
        await dbcontext.TextTable2MB.ExecuteDeleteAsync();

        dbcontext.TextTable2MB.Add(new TextTable2MB { Text = new string('x', 1024 * 1024 * 2) });
        dbcontext.TextTable5MB.Add(new TextTable5MB { Text = new string('x', 1024 * 1024 * 5) });

        await dbcontext.SaveChangesAsync();
    }

    [BenchmarkCategory("2MB"), Benchmark(Baseline = true)]
    public async Task Async2MB()
    {
        using var dbcontext = new SqlServerDbContext();

        _ = await dbcontext.TextTable2MB.FirstOrDefaultAsync();
    }

    [BenchmarkCategory("2MB"), Benchmark]
    public void Sync2MB()
    {
        using var dbcontext = new SqlServerDbContext();

        _ = dbcontext.TextTable2MB.FirstOrDefault();
    }

    [BenchmarkCategory("5MB"), Benchmark(Baseline = true)]
    public async Task Async5MB()
    {
        using var dbcontext = new SqlServerDbContext();

        _ = await dbcontext.TextTable5MB.FirstOrDefaultAsync();
    }

    [BenchmarkCategory("5MB"), Benchmark]
    public void Sync5MB()
    {
        using var dbcontext = new SqlServerDbContext();

        _ = dbcontext.TextTable5MB.FirstOrDefault();
    }

    //[Benchmark]
    //public async Task Async2MBPS()
    //{
    //    using var dbcontext = new TestDbContext("Server=localhost;Database=EFCoreLargeData;user id=sa;password=P@ssw0rd.123!;TrustServerCertificate=True;Packet Size=32767");

    //    _ = await dbcontext.TextTable2MBs.FirstOrDefaultAsync();
    //}

    //[Benchmark]
    //public void Sync2MBPS()
    //{
    //    using var dbcontext = new TestDbContext("Server=localhost;Database=EFCoreLargeData;user id=sa;password=P@ssw0rd.123!;TrustServerCertificate=True;Packet Size=32767");

    //    _ = dbcontext.TextTable2MBs.FirstOrDefault();
    //}
}
