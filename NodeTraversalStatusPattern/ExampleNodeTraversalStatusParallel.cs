namespace NodeTraversalStatusPattern
{
    using Utils;

    /// <summary>
    /// This class contains all pattern members to implement NodeTraversalStatusPattern
    /// </summary>
    public partial class ExampleNode
    {
        private ulong _nodeParallelTraversalState;

        public bool CheckSetProcessedParallel(NodeTraversalThreadContext state)
        {
            while (true)
            {
                var oldValue = _nodeParallelTraversalState;
                var newValue = oldValue;

                if (state.State)
                {
                    newValue |= state.StateBitValue;
                }
                else
                {
                    newValue ^= state.StateBitValue;
                }

                var prevValue = Interlocked.CompareExchange(ref _nodeParallelTraversalState, newValue, oldValue);

                if (prevValue == oldValue) //success write, value was updated
                {
                    var oldValueWasProcessed = (oldValue & state.StateBitMask) == state.StateBitValue;

                    return oldValueWasProcessed;
                }
            }
        }
    }
}
