namespace NodeTraversalStatusPattern.UnitTests
{
    using System.Reflection;
    using NodeTraversalStatusPattern.Utils;
    using Shouldly;
    using Traversal;
    using Traversal.StatusPatternGraphTraversal;
    using Utils;
    using Xunit;

    public class GraphTraversalUnitTest
    {
        private static FieldInfo? _threadCompletionStatesFieldInfo;
        private static FieldInfo? _threadTraversalStatesFieldInfo;

        public GraphTraversalUnitTest()
        {
            _threadCompletionStatesFieldInfo = typeof(NodeParallelUtils).GetField(
                "_threadCompletionStates",
                BindingFlags.Static | BindingFlags.NonPublic);

            _threadTraversalStatesFieldInfo = typeof(NodeParallelUtils).GetField(
                "_threadTraversalStates",
                BindingFlags.Static | BindingFlags.NonPublic);
        }

        [Theory]
        [InlineData(10, 10)]
        [InlineData(100, 100)]
        [InlineData(400, 400)]
        public void GraphTraversal_SingleThread_AllNodesProcessedOnce(int width, int height)
        {
            ClearStates();

            //Generate test graph
            var graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(width, height);
            var ssGraphTraversal = new SingleThreadStatusPatternGraphTraversal();

            const int STEPS = 3;

            //Do traversal (few times for a test, just to make sure everything is ok with states)
            for (var i = 0; i < STEPS; i++)
            {
                ssGraphTraversal.GraphTraversal(graphRootNode, node => node.IncrementValue());
            }

            //Validate nodes
            ValidateGraph(graphRootNode, STEPS);
        }

        [Theory]
        [InlineData(10, 10, 3)]
        [InlineData(100, 100, 10)]
        [InlineData(300, 300, 40)]
        public void GraphTraversal_MultiThread_AllNodesProcessedOnce(int width, int height, int threads)
        {
            ClearStates();

            //Generate test graph
            var graphRootNode = UnitTestsGraphProvider.GenerateGridGraph(width, height);

            const int STEPS = 3;

            var multithreadedTraversal = new MultithreadedStatusPatternGraphTraversal(threads);

            //Do traversal (few times for a test, just to make sure everything is ok with states)
            for (var i = 0; i < STEPS; i++)
            {
                multithreadedTraversal.GraphTraversal(graphRootNode, node => node.IncrementValue());
            }

            //Validate nodes
            ValidateGraph(graphRootNode, STEPS);
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
            ClearStates();

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
                        var parallelTraversal = new ParallelStatusPatternGraphTraversal(threads, threadContext);
                        parallelTraversal.GraphTraversal(graphRootNode, node => node.IncrementValue());
                        threadContext.Complete();
                    });
            }

            Task.WaitAll(tasks);

            //Validate nodes
            ValidateGraph(graphRootNode, parallelCalculations);
        }

        /// <summary>
        /// Clear thread states in cases when some tests/threads are not completed
        /// </summary>
        private static void ClearStates()
        {
            _threadCompletionStatesFieldInfo?.SetValue(null, 0uL);
            _threadTraversalStatesFieldInfo?.SetValue(null, 0uL);
        }

        private static void ValidateGraph(ExampleNode graphRootNode, int incremented)
        {
            var ssGraphTraversal = new SingleThreadStatusPatternGraphTraversal();
            ssGraphTraversal.GraphTraversal(graphRootNode, node => node.Value.ShouldBe(node.X + node.Y + incremented, $"Node:[{node.X},{node.Y}]"));
        }
    }
}
