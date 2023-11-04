using System.Diagnostics;

namespace TraversalStatePattern
{
    /// <summary>
    /// This class contains all logic and data related to his basic functionality
    /// </summary>
    [DebuggerDisplay("[{X},{Y}]={Value}")]
    public partial class ExampleNode
    {
        public ExampleNode(
            int value,
            int x,
            int y,
            int id)
        {
            Value = value;
            X = x;
            Y = y;
            Id = id;
        }

        public int Id { get; }

        public int Value
        {
            get => _value;
            private set => _value = value;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public void IncrementValue()
        {
            Interlocked.Increment(ref _value);
        }

        #region Graph hierarchy

        private readonly List<ExampleNode> _children = new();
        private int _value;
        public IReadOnlyList<ExampleNode> Children => _children;

        public void AddChild(ExampleNode child)
        {
            _children.Add(child);
        }

        #endregion
    }
}
