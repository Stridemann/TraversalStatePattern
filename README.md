# TraversalStatePattern
* [Introduction](#introduction)
* [Pattern code](#pattern-code)
* [Pattern usage](#pattern-usage)
* [Multithreading usage](#multithreading-usage)
* [Parallel multithreaded usage](#parallel-multithreaded-usage)

## Introduction
The TraversalStatePattern can be used to identify processed entities without requiring additional collections and to reset the IsProcessed flag after entities have been marked as processed during a full or partial traversal, eliminating the need to iterate through all nodes once again to clear the flag. It can be used in graph traversal or for processing other types of entities.

Example of a graph node to which we want to apply a pattern:
```cs
/// <summary>
/// This class contains all logic and data related to his basic functionality
/// </summary>
public partial class ExampleNode
{
	public readonly int Value;

	#region Graph hierarchy
	private readonly List<ExampleNode> _children = new();
	public IReadOnlyList<ExampleNode> Children => _children;

	public void AddChild(ExampleNode child)
	{
		_children.Add(child);
	}
	#endregion
}
```
## Pattern code

The idea is to include an integer state value within each Node, along with a global integer state used for comparison with the Node's state to determine whether the node has been processed. The Node's state value will be set to the global value to mark it as processed. Before each traversal, we increment the global state number, which effectively resets all nodes to an unprocessed state prior to the traversal.

By using a partial class, we can relocate all pattern logic to a separate file:
```cs
/// <summary>
/// This class contains all pattern members necessary to implement TraversalStatePattern
/// </summary>
public partial class ExampleNode
{
    private static int _globalTraversalState;
    private int _nodeTraversalState = -1;//set to -1 to not be IsProcessed() by default
   
    /// <summary>
    /// Called before new traversal to make IsProcessed() for all nodes to be False
    /// </summary>
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
```
We can combine the MarkProcessed() and IsProcessed() methods into one. It will only return 'True' the first time you use it:
```cs
public bool CheckSetProcessed()
{
    if (_nodeTraversalState == _globalTraversalState) //check is processed
        return true;
    _nodeTraversalState = _globalTraversalState;

    return false;
}
```

## Pattern usage
Example graph traversal method:
```cs
public void GraphTraversal(ExampleNode root, Action<ExampleNode> procedure)
{
    //Start a new traversal. Now all nodes IsProcessed() will return False after this
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
```

## Multithreading usage
For multithreading processing we should modify the code a bit:
```cs
public partial class ExampleNode
{
    private static int _globalTraversalState;
    private int _nodeTraversalState = -1;

    public static void NewTraversal()
    {
        _globalTraversalState++;
    }

    public bool CheckSetProcessed()
    {
        var newState = Interlocked.Exchange(ref _nodeTraversalState, _globalTraversalState);

        //if the original value was now equal to global, then node was not processed
        return _globalTraversalState == newState;
    }
}
```

Here's an example of how to use it in code that runs in multiple threads. I used BackgroundWorkers, but you can use other methods like Tasks.
```cs
public void GraphTraversal(
    ExampleNode root,
    Action<ExampleNode> procedure,
    int threads,
    int nodes)
{
    ExampleNode.NewTraversalMultithreaded(); //Start a new traversal. Now all nodes IsProcessed() will give False

    //Do multithreaded traversal
    var waitEvent = new CountdownEvent(threads);

    var processingBC = new BlockingCollection<ExampleNode> { root };

    for (var i = 0; i < threads; i++)
    {
        var worker = new BackgroundWorker();

        worker.DoWork += (_, _) =>
        {
            foreach (var exampleNode in processingBC.GetConsumingEnumerable())
            {
                procedure(exampleNode);

                if (Interlocked.Decrement(ref nodes) == 0)
                    processingBC.CompleteAdding();

                foreach (var child in exampleNode.Children)
                {
                    if (child.CheckSetProcessedMultithreaded())
                    {
                        continue;
                    }

                    processingBC.Add(child);
                }
            }
        };

        worker.RunWorkerCompleted += (_, _) =>
        {
            waitEvent.Signal();
            worker.Dispose();
        };
        worker.RunWorkerAsync();
    }

    waitEvent.Wait();
}
```

## Parallel multithreaded usage
(under Parallel multithreaded means doing multiple concurrent iterations over the same structure, where each iteration runs using multithreading)

The repository has a parallel, multithreaded example of how to use it, but the benchmarks show it's only 5-10% faster than using ConcurrentDictionary.
[ConcurrentDictionary](TraversalStatePattern.TraversalUtils/DefaultGraphTraversal/MultithreadedDictionaryGraphTraversal.cs)

Parallel multithreaded way without additional collections, with bits manipulation and limitation of 64 parallel operations:
[ParallelStatePatternGraphTraversal.cs](TraversalStatePattern.TraversalUtils/StatePatternTraversal/ParallelStatePatternGraphTraversal.cs)

[NodeParallelUtils.cs](TraversalStatePattern/Utils/NodeParallelUtils.cs) (provides thread traversal context for tracking flag bits).
