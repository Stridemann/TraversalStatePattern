namespace NodeTraversalStatusPattern.Traversal.StatusPatternGraphTraversal
{
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using Utils;

    public class ParallelStatusPatternGraphTraversal : IGraphTraversal
    {
        private readonly int _threads;
        private readonly NodeTraversalThreadContext _threadContext;

        public ParallelStatusPatternGraphTraversal(int threads, NodeTraversalThreadContext threadContext)
        {
            _threads = threads;
            _threadContext = threadContext;
        }

        #region Implementation of IGraphTraversal

        public void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure)
        {
            var waitEvent = new CountdownEvent(_threads);
            var queue = new ConcurrentQueue<ExampleNode>();

            //process initial node
            procedure(root);

            foreach (var exampleNode in root.Children)
            {
                queue.Enqueue(exampleNode);
            }

            //Do multithreaded traversal
            for (var i = 0; i < _threads; i++)
            {
                var worker = new BackgroundWorker();

                worker.DoWork += (_, _) =>
                {
                    while (queue.TryDequeue(out var recursionNode))
                    {
                        if (recursionNode.CheckSetProcessedParallel(_threadContext))
                        {
                            continue;
                        }

                        procedure(recursionNode);

                        foreach (var child in recursionNode.Children)
                        {
                            queue.Enqueue(child);
                        }
                    }
                };
                worker.RunWorkerAsync();

                worker.RunWorkerCompleted += (_, _) =>
                {
                    waitEvent.Signal();
                    worker.Dispose();
                };
            }

            waitEvent.Wait();
        }

        #endregion
    }
}
