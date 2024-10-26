using BenchmarkDotNet.Running;
using SqlServerProvider;

Console.WriteLine(BenchmarkRunner.Run<EFSqlServerBenchmark>());