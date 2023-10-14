namespace NodeTraversalStatusPattern.UnitTests
{
    using System.Reflection;
    using NodeTraversalStatusPattern.Utils;
    using Shouldly;
    using Xunit;

    public class NodeParallelUtilsUnitTest
    {
        public NodeParallelUtilsUnitTest()
        {
            typeof(NodeParallelUtils).GetField("_threadCompletionStates", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, 0uL);
            typeof(NodeParallelUtils).GetField("_threadTraversalStates", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, 0uL);
        }

        [Theory]
        [InlineData(
            1,
            0,
            true,
            1uL,
            1uL)]
        [InlineData(
            2,
            1,
            true,
            1uL << 1,
            1uL << 1)]
        [InlineData(
            63,
            62,
            true,
            1uL << 62,
            1uL << 62)]
        public void NodeParallelUtils_GetNewThreadContext_ReturnCorrectValues(
            int contextsNum,
            int expectedFreeThreadIndex,
            bool expectedState,
            ulong expectedBitMask,
            ulong expectedBitValue)
        {
            NodeTraversalThreadContext threadContext = default;
            var freeThreadId = 0;

            for (int i = 0; i < contextsNum; i++)
            {
                freeThreadId = NodeParallelUtils.GetFreeThreadIndex();

                if (freeThreadId == -1)
                    throw new InvalidOperationException("No free threads to do traversal");

                threadContext = NodeParallelUtils.GetNewThreadContext(freeThreadId);
            }

            freeThreadId.ShouldBe(expectedFreeThreadIndex);
            threadContext.State.ShouldBe(expectedState);
            threadContext.StateBitMask.ShouldBe(expectedBitMask);
            threadContext.StateBitValue.ShouldBe(expectedBitValue);
        }

        [Fact]
        public void NodeParallelUtils_GetNewThreadContext_NoFreeThread()
        {
            for (int i = 0; i < 65; i++)
            {
                var freeThreadId = NodeParallelUtils.GetFreeThreadIndex();

                if (i == 64)
                    freeThreadId.ShouldBe(-1);
                else
                    NodeParallelUtils.GetNewThreadContext(freeThreadId);
            }
        }

        [Theory]
        [InlineData(
            2,
            0,
            false,
            1uL,
            0uL)]
        public void NodeParallelUtils_GetNewThreadContextAfterComplete_ReturnCorrectValues(
            int contextsNum,
            int expectedFreeThreadIndex,
            bool expectedState,
            ulong expectedBitMask,
            ulong expectedBitValue)
        {
            NodeTraversalThreadContext threadContext = default;
            var freeThreadId = 0;

            for (int i = 0; i < contextsNum; i++)
            {
                freeThreadId = NodeParallelUtils.GetFreeThreadIndex();

                if (freeThreadId == -1)
                    throw new InvalidOperationException("No free threads to do traversal");

                threadContext = NodeParallelUtils.GetNewThreadContext(freeThreadId);
                threadContext.Complete();
            }

            freeThreadId.ShouldBe(expectedFreeThreadIndex);
            threadContext.State.ShouldBe(expectedState);
            threadContext.StateBitMask.ShouldBe(expectedBitMask);
            threadContext.StateBitValue.ShouldBe(expectedBitValue);
        }
    }
}
