namespace NodeTraversalStatusPattern
{
    /// <summary>
    /// This class contains all pattern members to implement NodeTraversalStatusPattern
    /// </summary>
    public partial class ExampleNode
    {
        [ThreadStatic]
        private static int _globalMultithreadedTraversalState;

        private int _nodeMultithreadedTraversalState = -1; //set to -1 to not be IsProcessed() by default

        public static void NewTraversalMultithreaded()
        {
            _globalMultithreadedTraversalState++;
        }

        public bool CheckSetProcessedMultithreaded()
        {
            var newState = Interlocked.Exchange(
                ref _nodeMultithreadedTraversalState,
                _globalMultithreadedTraversalState);

            //if the original value was now equal to global, then node was not processed
            return _globalMultithreadedTraversalState == newState;
        }
    }
}
