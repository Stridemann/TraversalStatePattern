namespace TraversalStatePattern
{
    /// <summary>
    /// This class contains all pattern members to implement NodeTraversalStatusPattern
    /// </summary>
    public partial class ExampleNode
    {
        private static int _globalTraversalState;
        private int _nodeTraversalState = -1; //set to -1 to not be IsProcessed() by default

        /// <summary>
        /// Called before traversal to make IsProcessed() for all nodes to be False
        /// </summary>
        public static void NewTraversal()
        {
            _globalTraversalState++;
        }

        public bool CheckSetProcessed()
        {
            if (_nodeTraversalState == _globalTraversalState) //check is processed
                return true;
            _nodeTraversalState = _globalTraversalState;

            return false;
        }
    }
}
