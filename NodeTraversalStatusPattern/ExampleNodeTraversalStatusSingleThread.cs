namespace NodeTraversalStatusPattern
{
    /// <summary>
    /// This class contains all pattern members to implement NodeTraversalStatusPattern
    /// </summary>
    public partial class ExampleNode
    {
        private static int _globalTraversalState;
        private int _nodeTraversalState = -1;//set to -1 to not be IsProcessed() by default

        public static void NewTraversal()
        {
            _globalTraversalState++;
        }

        public void MarkProcessed()
        {
            _nodeTraversalState = _globalTraversalState;
        }

        public bool IsProcessed() => _nodeTraversalState == _globalTraversalState;
    }
}
