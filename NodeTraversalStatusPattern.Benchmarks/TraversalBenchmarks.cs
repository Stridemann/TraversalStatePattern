namespace NodeTraversalStatusPattern.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [InProcess]
    public class TraversalBenchmarks
    {
        [IterationSetup]
        public void GlobalSetup()
        {
           //init
        }

        [Benchmark]
        public void BenchmarkSegmentation()
        {

        }
    }
}
