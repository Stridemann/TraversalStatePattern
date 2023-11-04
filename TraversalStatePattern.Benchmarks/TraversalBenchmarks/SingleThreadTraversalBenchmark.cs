using System.Reflection;
using BenchmarkDotNet.Attributes;
using TraversalStatePattern.TraversalUtils.DefaultGraphTraversal;
using TraversalStatePattern.TraversalUtils.StatePatternTraversal;
using TraversalStatePattern.UnitTests.Utils;
using TraversalStatePattern.Utils;

namespace TraversalStatePattern.Benchmarks.TraversalBenchmarks
{
    [InProcess]
    public class SingleThreadStatusPatternTraversalBenchmark
    {
        private ExampleNode? _graphRootNode;

        [IterationSetup]
        public void GlobalSetup()
        {
            _graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(300, 300);

            typeof(NodeParallelUtils).GetField(
                                         "_threadCompletionStates",
                                         BindingFlags.Static | BindingFlags.NonPublic)!
                                     .SetValue(null, 0uL);
        }

        [Benchmark]
        public void BenchmarkSingleThreadStatusPattern()
        {
            var ssGraphTraversal = new SingleThreadStatePatternGraphTraversal();
            ssGraphTraversal.GraphTraversal(_graphRootNode, node => node.IncrementValue());
        }
    }

    [InProcess]
    public class SingleThreadTraversalBenchmark
    {
        private ExampleNode? _graphRootNode;

        [IterationSetup]
        public void GlobalSetup()
        {
            _graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(300, 300);

            typeof(NodeParallelUtils).GetField(
                                         "_threadCompletionStates",
                                         BindingFlags.Static | BindingFlags.NonPublic)!
                                     .SetValue(null, 0uL);
        }

        [Benchmark]
        public void BenchmarkSingleThreadStatusPattern()
        {
            var ssGraphTraversal = new SingleThreadHashsetGraphTraversal();
            ssGraphTraversal.GraphTraversal(_graphRootNode, node => node.IncrementValue());
        }
    }
}
