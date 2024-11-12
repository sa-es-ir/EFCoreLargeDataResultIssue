using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace PostgresqlProvider;

[MemoryDiagnoser(false)]
[Config(typeof(Config))]
[HideColumns(Column.RatioSD, Column.AllocRatio)]
[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[ShortRunJob]
public class EFPostgresBenchmark
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
        var dbcontext = new PostgresqlDbContext();

        await dbcontext.TextTable2MB.ExecuteDeleteAsync();
        await dbcontext.TextTable5MB.ExecuteDeleteAsync();

        dbcontext.TextTable2MB.Add(new TextTable2MB { Text = new string('x', 1024 * 1024 * 2) });
        dbcontext.TextTable5MB.Add(new TextTable5MB { Text = new string('x', 1024 * 1024 * 5) });

        await dbcontext.SaveChangesAsync();
    }

    [BenchmarkCategory("2MB"), Benchmark(Baseline = true)]
    public async Task Async2MB()
    {
        using var dbcontext = new PostgresqlDbContext();

        _ = await dbcontext.TextTable2MB.FirstOrDefaultAsync();
    }

    [BenchmarkCategory("2MB"), Benchmark]
    public void Sync2MB()
    {
        using var dbcontext = new PostgresqlDbContext();

        _ = dbcontext.TextTable2MB.FirstOrDefault();
    }

    [BenchmarkCategory("5MB"), Benchmark(Baseline = true)]
    public async Task Async5MB()
    {
        using var dbcontext = new PostgresqlDbContext();

        _ = await dbcontext.TextTable5MB.FirstOrDefaultAsync();
    }

    [BenchmarkCategory("5MB"), Benchmark]
    public void Sync5MB()
    {
        using var dbcontext = new PostgresqlDbContext();

        _ = dbcontext.TextTable5MB.FirstOrDefault();
    }
}
