namespace NodeTraversalStatusPattern.Traversal
{
    public interface IGraphTraversal
    {
        void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure);
    }
}
