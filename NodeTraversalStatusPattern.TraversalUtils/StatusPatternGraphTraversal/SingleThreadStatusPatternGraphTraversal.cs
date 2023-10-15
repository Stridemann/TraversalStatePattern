namespace NodeTraversalStatusPattern.Traversal.StatusPatternGraphTraversal
{
    public class SingleThreadStatusPatternGraphTraversal : IGraphTraversal
    {
        public void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure)
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
                    {
                        continue;
                    }

                    resultChild.MarkProcessed();

                    //Do some work
                    procedure(resultChild);

                    //process node childen
                    queue.Enqueue(resultChild);
                }
            }
        }
    }
}
