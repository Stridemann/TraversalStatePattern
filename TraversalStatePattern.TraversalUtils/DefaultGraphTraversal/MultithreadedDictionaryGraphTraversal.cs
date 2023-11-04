using System.Collections.Concurrent;
using System.ComponentModel;

namespace TraversalStatePattern.Traversal.DefaultGraphTraversal
{
    public class MultithreadedDictionaryGraphTraversal
    {
        private readonly int _threads;
        private readonly ConcurrentDictionary<int, byte> _duplicatesDict = new ConcurrentDictionary<int, byte>();

        public MultithreadedDictionaryGraphTraversal(int threads)
        {
            _threads = threads;
        }

        #region Implementation of IGraphTraversal

        public void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure)
        {
            //Do multithreaded traversal
            var waitEvent = new CountdownEvent(_threads);
            var queue = new ConcurrentQueue<ExampleNode>();

            //process initial node
            procedure(root);
            _duplicatesDict[root.Id] = 0;

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
                        if (!_duplicatesDict.TryAdd(recursionNode.Id, 0))
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
