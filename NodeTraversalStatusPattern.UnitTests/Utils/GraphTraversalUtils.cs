namespace NodeTraversalStatusPattern.UnitTests.Utils
{
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using NodeTraversalStatusPattern.Utils;

    public static class GraphTraversalUtils
    {
        public static void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure)
        {
            //Start a new traversal. Now all nodes IsProcessed() will give False
            ExampleNode.NewTraversal();

            //Using Queue for BFS (or Stack can be used for DFS traversal)
            var queue = new Queue<ExampleNode>();
            queue.Enqueue(root);
            root.MarkProcessed();

            while (queue.TryDequeue(out var node))
            {
                foreach (var resultChild in node.Children)
                {
                    //Use status pattern
                    if (resultChild.IsProcessed())
                        continue;
                    resultChild.MarkProcessed();

                    //Do some work
                    procedure(resultChild);

                    //process node childen
                    queue.Enqueue(resultChild);
                }
            }
        }

        public static void GraphTraversalMultithreaded(ExampleNode node, Action<ExampleNode> procedure, int threads)
        {
            ExampleNode.NewTraversalMultithreaded(); //Start a new traversal. Now all nodes IsProcessed() will give False

            node.CheckSetProcessedMultithreaded();

            //Do multithreaded traversal
            var waitEvent = new CountdownEvent(threads);
            var queue = new ConcurrentQueue<ExampleNode>();
            //process initial node
            procedure(node);

            foreach (var exampleNode in node.Children)
            {
                queue.Enqueue(exampleNode);
            }

            for (int i = 0; i < threads; i++)
            {
                var worker = new BackgroundWorker();
                worker.WorkerSupportsCancellation = true;

                worker.DoWork += (_, _) =>
                {
                    while (queue.TryDequeue(out var recursionNode))
                    {
                        if (recursionNode.CheckSetProcessedMultithreaded())
                            continue;
                        procedure(recursionNode);

                        foreach (var child in recursionNode.Children)
                            queue.Enqueue(child);
                    }
                };
                worker.RunWorkerAsync();
                worker.RunWorkerCompleted += (_, _) => waitEvent.Signal();
            }

            waitEvent.Wait();
        }

        public static void GraphTraversalParallel(
            ExampleNode node,
            Action<ExampleNode> procedure,
            int threads,
            NodeTraversalThreadContext threadContext)
        {
            var waitEvent = new CountdownEvent(threads);
            var queue = new ConcurrentQueue<ExampleNode>();

            //process initial node
            procedure(node);

            foreach (var exampleNode in node.Children)
            {
                queue.Enqueue(exampleNode);
            }

            //Do multithreaded traversal
            for (int i = 0; i < threads; i++)
            {
                var worker = new BackgroundWorker();
                worker.WorkerSupportsCancellation = true;

                worker.DoWork += (_, _) =>
                {
                    while (queue.TryDequeue(out var recursionNode))
                    {
                        if (recursionNode.CheckSetProcessedParallel(threadContext))
                            continue;

                        procedure(recursionNode);

                        foreach (var child in recursionNode.Children)
                            queue.Enqueue(child);
                    }
                };
                worker.RunWorkerAsync();
                worker.RunWorkerCompleted += (_, _) => { waitEvent.Signal(); };
            }

            waitEvent.Wait();
        }
    }
}
