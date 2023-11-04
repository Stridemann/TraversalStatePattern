namespace TraversalStatePattern.TraversalUtils.DefaultGraphTraversal
{
    public class SingleThreadHashsetGraphTraversal
    {
        public void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure)
        {
            var visitedNodes = new HashSet<int>();
            //Using Queue for BFS (or Stack can be used for DFS traversal)
            var queue = new Queue<ExampleNode>();
            queue.Enqueue(root);

            while (queue.TryDequeue(out var node))
            {
                foreach (var resultChild in node.Children)
                {
                    if (visitedNodes.Add(resultChild.Id))
                    {
                        visitedNodes.Add(resultChild.Id);
                        //Do some work
                        procedure(resultChild);

                        //process node childen
                        queue.Enqueue(resultChild);
                    }
                }
            }
        }
    }
}
