namespace NodeTraversalStatusPattern.UnitTests
{
    using System.Reflection;
    using NodeTraversalStatusPattern.Utils;
    using Shouldly;
    using Utils;
    using Xunit;

    public class GraphTraversalUnitTest
    {
        public GraphTraversalUnitTest()
        {
            typeof(NodeParallelUtils).GetField("_threadCompletionStates", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, 0uL);
            typeof(NodeParallelUtils).GetField("_threadTraversalStates", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, 0uL);
        }

        [Theory]
        [InlineData(10, 10)]
        [InlineData(100, 100)]
        [InlineData(400, 400)]
        public void GraphTraversal_SingleThread_AllNodesProcessedOnce(int width, int height)
        {
            //Generate test graph
            var graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(width, height);

            //Do traversal
            GraphTraversalUtils.GraphTraversal(graphRootNode, node => node.IncrementValue());

            //Validate nodes
            GraphTraversalUtils.GraphTraversal(graphRootNode, node => node.Value.ShouldBe(node.X + node.Y + 1));
        }

        [Theory]
        [InlineData(10, 10, 3)]
        [InlineData(100, 100, 10)]
        [InlineData(400, 400, 40)]
        public void GraphTraversal_MultiThread_AllNodesProcessedOnce(int width, int height, int threads)
        {
            //Generate test graph
            var graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(width, height);

            //Do traversal (few times for a test, just to make sure everything is ok with states)
            for (var i = 0; i < 3; i++)
            {
                GraphTraversalUtils.GraphTraversalMultithreaded(graphRootNode, node => node.IncrementValue(), threads);
            }

            //Validate nodes
            GraphTraversalUtils.GraphTraversal(graphRootNode, node => node.Value.ShouldBe(node.X + node.Y + 1, $"Node:[{node.X},{node.Y}]"));
        }

        [Theory]
        [InlineData(10, 10, 30, 20)] //Do not set high values in parallel/threads, unit tests will fail to start some threads
        [InlineData(100, 100, 30, 20)]
        [InlineData(400, 400, 10, 30)]
        public void GraphTraversal_MultiThreadParallelCalculaions_AllNodesProcessedOnce(
            int width,
            int height,
            int parallelCalculations,
            int threads)
        {
            //Generate test graph
            var graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(width, height);

            //Do traversal
            var tasks = new Task[parallelCalculations];

            for (var i = 0; i < parallelCalculations; i++)
            {
                var freeThreadId = NodeParallelUtils.GetFreeThreadIndex();

                if (freeThreadId == -1)
                {
                    throw new InvalidOperationException("No free threads to do traversal");
                }

                var threadContext = NodeParallelUtils.GetNewThreadContext(freeThreadId);

                tasks[i] = Task.Run(
                    () =>
                    {
                        GraphTraversalUtils.GraphTraversalParallel(
                            graphRootNode,
                            node => node.IncrementValue(),
                            threads,
                            threadContext);
                        threadContext.Complete();
                    });
            }

            Task.WaitAll(tasks);

            //Validate nodes
            GraphTraversalUtils.GraphTraversal(
                graphRootNode,
                node => node.Value.ShouldBe(node.X + node.Y + parallelCalculations, $"Node:[{node.X},{node.Y}]"));
        }
    }
}
