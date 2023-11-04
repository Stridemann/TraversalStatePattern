using System.Collections.Concurrent;
using System.ComponentModel;

namespace TraversalStatePattern.TraversalUtils.StatePatternTraversal
{
    public class MultithreadedStatePatternGraphTraversal
    {
        #region Implementation of IGraphTraversal

        public void GraphTraversal(
            ExampleNode root,
            Action<ExampleNode> procedure,
            int threads,
            int nodes)
        {
            ExampleNode.NewTraversalMultithreaded(); //Start a new traversal. Now all nodes IsProcessed() will give False

            //Do multithreaded traversal
            var waitEvent = new CountdownEvent(threads);

            var processingBC = new BlockingCollection<ExampleNode> { root };

            for (var i = 0; i < threads; i++)
            {
                var worker = new BackgroundWorker();

                worker.DoWork += (_, _) =>
                {
                    foreach (var exampleNode in processingBC.GetConsumingEnumerable())
                    {
                        procedure(exampleNode);

                        if (Interlocked.Decrement(ref nodes) == 0)
                            processingBC.CompleteAdding();

                        foreach (var child in exampleNode.Children)
                        {
                            if (child.CheckSetProcessedMultithreaded())
                            {
                                continue;
                            }

                            processingBC.Add(child);
                        }
                    }
                };

                worker.RunWorkerCompleted += (_, _) =>
                {
                    waitEvent.Signal();
                    worker.Dispose();
                };
                worker.RunWorkerAsync();
            }

            waitEvent.Wait();
        }

        #endregion
    }
}
