using System.Collections.Concurrent;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using ProtoBenchmarkHelpers.ProtoBenchmarkHelpers;
using Shouldly;
using TraversalStatePattern.TraversalUtils.StatePatternTraversal;
using TraversalStatePattern.UnitTests.Utils;
using TraversalStatePattern.Utils;

namespace TraversalStatePattern.Benchmarks
{
    public class AsyncTraversalBenchmarkRunner
    {
        private const int numTasks = 30;

        private AsyncBenchmarkThreadHelper asyncThreadHelper;
        private BlockingCollection<ExampleNode> _processingBc;
        private int _nodes;
        private ExampleNode _graphRootNode;

        [GlobalSetup(Target = nameof(AsyncThreadHelper))]
        public void SetupAsyncThreadHelper()
        {
            typeof(NodeParallelUtils).GetField(
                                         "_threadCompletionStates",
                                         BindingFlags.Static | BindingFlags.NonPublic)!
                                     .SetValue(null, 0uL);

            var width = 300;
            var height = 300;
            _graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(width, height);
            _nodes = width * height;
            _processingBc = new BlockingCollection<ExampleNode> { _graphRootNode };

            asyncThreadHelper = new AsyncBenchmarkThreadHelper();

            for (int i = 0; i < numTasks; ++i)
            {
                asyncThreadHelper.Add(FuncAsync);
            }
        }

        private async ValueTask FuncAsync()
        {
            foreach (var exampleNode in _processingBc.GetConsumingEnumerable())
            {
                exampleNode.IncrementValue();

                if (Interlocked.Decrement(ref _nodes) == 0)
                    _processingBc.CompleteAdding();

                foreach (var child in exampleNode.Children)
                {
                    if (child.CheckSetProcessedMultithreaded())
                    {
                        continue;
                    }

                    _processingBc.Add(child);
                }
            }

            var ssGraphTraversal = new SingleThreadStatePatternGraphTraversal();
            ssGraphTraversal.GraphTraversal(_graphRootNode, node => node.Value.ShouldBe(node.X + node.Y + 2, $"Node:[{node.X},{node.Y}]"));
        }

        [Benchmark]
        public ValueTask AsyncThreadHelper()
        {
            return asyncThreadHelper.ExecuteAndWaitAsync();
        }
    }
}
