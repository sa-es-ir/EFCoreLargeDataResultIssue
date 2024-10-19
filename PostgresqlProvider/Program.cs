using BenchmarkDotNet.Running;
using PostgresqlProvider;

Console.WriteLine(BenchmarkRunner.Run<EFBenchmark>());
