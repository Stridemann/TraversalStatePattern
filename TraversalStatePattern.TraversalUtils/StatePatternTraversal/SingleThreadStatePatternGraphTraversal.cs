namespace TraversalStatePattern.TraversalUtils.StatePatternTraversal
{
    public class SingleThreadStatePatternGraphTraversal
    {
        public void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure)
        {
            //Start a new traversal. Now all nodes IsProcessed() will give False
            ExampleNode.NewTraversal();

            //Using Queue for BFS (or Stack can be used for DFS traversal)
            var queue = new Queue<ExampleNode>();

            //process start node
            queue.Enqueue(root);
            root.CheckSetProcessed();

            while (queue.TryDequeue(out var node))
            {
                foreach (var resultChild in node.Children)
                {
                    //Use status pattern
                    if (resultChild.CheckSetProcessed())
                    {
                        continue;
                    }

                    //Do some work
                    procedure(resultChild);

                    //process node children
                    queue.Enqueue(resultChild);
                }
            }
        }
    }
}
