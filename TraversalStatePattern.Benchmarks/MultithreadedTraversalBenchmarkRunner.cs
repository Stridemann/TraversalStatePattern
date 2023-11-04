using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using TraversalStatePattern.Benchmarks.TraversalBenchmarks;
using Xunit;
using Xunit.Abstractions;

namespace TraversalStatePattern.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class MultithreadedTraversalBenchmarkRunner
    {
        public MultithreadedTraversalBenchmarkRunner(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        [Fact]
        public void MultithreadedStatusPatternGraphTraversalBenchmark()
        {
            var logger = new AccumulationLogger();

            var config = ManualConfig.Create(DefaultConfig.Instance)
                                     .AddLogger(logger)
                                     .WithOptions(ConfigOptions.DisableOptimizationsValidator);

            BenchmarkRunner.Run<MultithreadedStatusPatternGraphTraversalBenchmark>(config);

            // write benchmark summary
            _output.WriteLine(logger.GetLog());
        }

        [Fact]
        public void MultithreadedDictionaryGraphTraversalBenchmark()
        {
            var logger = new AccumulationLogger();

            var config = ManualConfig.Create(DefaultConfig.Instance)
                                     .AddLogger(logger)
                                     .WithOptions(ConfigOptions.DisableOptimizationsValidator);

            BenchmarkRunner.Run<MultithreadedDictionaryGraphTraversalBenchmark>(config);

            // write benchmark summary
            _output.WriteLine(logger.GetLog());
        }

        [Fact]
        public void AsyncTraversalBenchmarkRunner()
        {
            var logger = new AccumulationLogger();

            var config = ManualConfig.Create(DefaultConfig.Instance)
                                     .AddLogger(logger)
                                     .WithOptions(ConfigOptions.DisableOptimizationsValidator);

            BenchmarkRunner.Run<AsyncTraversalBenchmarkRunner>(config);

            // write benchmark summary
            _output.WriteLine(logger.GetLog());
        }
    }
}
