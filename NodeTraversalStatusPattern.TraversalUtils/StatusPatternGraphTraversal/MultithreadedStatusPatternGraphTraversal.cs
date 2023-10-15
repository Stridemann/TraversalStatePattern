namespace NodeTraversalStatusPattern.Traversal.StatusPatternGraphTraversal
{
    using System.Collections.Concurrent;
    using System.ComponentModel;

    public class MultithreadedStatusPatternGraphTraversal : IGraphTraversal
    {
        private readonly int _threads;

        public MultithreadedStatusPatternGraphTraversal(int threads)
        {
            _threads = threads;
        }

        #region Implementation of IGraphTraversal

        public void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure)
        {
            ExampleNode.NewTraversalMultithreaded(); //Start a new traversal. Now all nodes IsProcessed() will give False

            //Do multithreaded traversal
            var waitEvent = new CountdownEvent(_threads);
            var queue = new ConcurrentQueue<ExampleNode>();

            //process initial node
            procedure(root);
            root.CheckSetProcessedMultithreaded();

            foreach (var exampleNode in root.Children)
            {
                queue.Enqueue(exampleNode);
            }

            for (var i = 0; i < _threads; i++)
            {
                var worker = new BackgroundWorker();

                worker.DoWork += (_, _) =>
                {
                    while (queue.TryDequeue(out var recursionNode))
                    {
                        if (recursionNode.CheckSetProcessedMultithreaded())
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
