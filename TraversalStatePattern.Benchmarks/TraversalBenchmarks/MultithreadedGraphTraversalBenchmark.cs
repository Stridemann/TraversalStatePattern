using System.Reflection;
using BenchmarkDotNet.Attributes;
using TraversalStatePattern.Traversal.DefaultGraphTraversal;
using TraversalStatePattern.TraversalUtils.StatePatternTraversal;
using TraversalStatePattern.UnitTests.Utils;
using TraversalStatePattern.Utils;

namespace TraversalStatePattern.Benchmarks.TraversalBenchmarks
{
    [InProcess]
    public class MultithreadedStatusPatternGraphTraversalBenchmark
    {
        private ExampleNode? _graphRootNode;

        [IterationSetup]
        public void GlobalSetup()
        {
            _graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(400, 400);

            typeof(NodeParallelUtils).GetField(
                                         "_threadCompletionStates",
                                         BindingFlags.Static | BindingFlags.NonPublic)!
                                     .SetValue(null, 0uL);
        }

        [Benchmark]
        public void BenchmarkSingleThreadStatusPattern()
        {
            var ssGraphTraversal = new MultithreadedStatePatternGraphTraversal();
            ssGraphTraversal.GraphTraversal(_graphRootNode, node => node.IncrementValue(), 30, 400 * 400);
        }
    }

    [InProcess]
    public class MultithreadedDictionaryGraphTraversalBenchmark
    {
        private ExampleNode? _graphRootNode;

        [IterationSetup]
        public void GlobalSetup()
        {
            _graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(400, 400);

            typeof(NodeParallelUtils).GetField(
                                         "_threadCompletionStates",
                                         BindingFlags.Static | BindingFlags.NonPublic)!
                                     .SetValue(null, 0uL);
        }

        [Benchmark]
        public void BenchmarkSingleThreadStatusPattern()
        {
            var ssGraphTraversal = new MultithreadedDictionaryGraphTraversal(30);
            ssGraphTraversal.GraphTraversal(_graphRootNode, node => node.IncrementValue());
        }
    }
}
