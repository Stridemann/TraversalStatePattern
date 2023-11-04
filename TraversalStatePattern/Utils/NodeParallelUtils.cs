namespace TraversalStatePattern.Utils
{
    public static class NodeParallelUtils
    {
        private static ulong _threadCompletionStates;
        private static ulong _threadTraversalStates;

        /// <summary>
        /// Returns -1 if free thread is not found
        /// </summary>
        public static int GetFreeThreadIndex()
        {
            for (var i = 0; i < 64; i++)
            {
                var bitMask = 1uL << i;

                if ((_threadCompletionStates & bitMask) == 0) //frind free thread index
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Non thread safe, should be called in one thread.
        /// Don't forget to call ThreadTraversalState.Complete() on end of processing
        /// </summary>
        public static NodeTraversalThreadContext GetNewThreadContext(int threadIndex)
        {
            if (threadIndex is < 0 or >= 64)
            {
                throw new ArgumentOutOfRangeException(nameof(threadIndex));
            }


            ulong bitMask = 1uL << threadIndex;
            ulong mask = bitMask;
            _threadCompletionStates |= mask;
            _threadTraversalStates ^= bitMask; //flip processing state flag
            var stateBitValue = _threadTraversalStates & bitMask;
            var state = stateBitValue > 0;

            return new NodeTraversalThreadContext(state, stateBitValue, bitMask, threadIndex);
        }

        internal static void OnThreadComplete(int threadIndex)
        {
            var bitMask = 1uL << threadIndex;
            _threadCompletionStates &= ~bitMask;
        }
    }

    public readonly struct NodeTraversalThreadContext
    {
        public readonly bool State;
        public readonly ulong StateBitValue;
        public readonly ulong StateBitMask;
        private readonly int _threadIndex;

        public NodeTraversalThreadContext(
            bool state,
            ulong stateBitValue,
            ulong stateMask,
            int threadIndex)
        {
            State = state;
            StateBitValue = stateBitValue;
            StateBitMask = stateMask;
            _threadIndex = threadIndex;
        }

        public void Complete()
        {
            NodeParallelUtils.OnThreadComplete(_threadIndex);
        }
    }
}
