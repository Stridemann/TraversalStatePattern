namespace NodeTraversalStatusPattern.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Running;
    using Xunit;
    using Xunit.Abstractions;

    [MemoryDiagnoser]
    [InProcess]
    public class AlgorithmBenchmarkRunner
    {
        private readonly ITestOutputHelper _output;

        public AlgorithmBenchmarkRunner(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TraversalBenchmark()
        {
            var logger = new AccumulationLogger();

            var config = ManualConfig.Create(DefaultConfig.Instance)
                                     .AddLogger(logger)
                                     .WithOptions(ConfigOptions.DisableOptimizationsValidator);

            BenchmarkRunner.Run<TraversalBenchmarks>(config);

            // write benchmark summary
            _output.WriteLine(logger.GetLog());
        }
    }
}
