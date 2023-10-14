namespace NodeTraversalStatusPattern.UnitTests.Utils
{
    public static class UnitTestsGraphProvider
    {
        public static ExampleNode GenerateGridGraph(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            var gridGraph = new ExampleNode[height][];

            //create grid nodes
            for (int y = 0; y < height; y++)
            {
                var rowNodes = new ExampleNode[width];
                gridGraph[y] = rowNodes;

                for (int x = 0; x < width; x++)
                {
                    var nodeValue = x + y;
                    rowNodes[x] = new ExampleNode(nodeValue, x, y);
                }
            }

            //Link bottom node all nodes to the right
            for (int y = 0; y < height - 1; y++)
            {
                var rowNodes = gridGraph[y];
                var nextRowNodes = gridGraph[y + 1];

                for (int x = 0; x < width - 1; x++)
                {
                    var node = rowNodes[x];

                    for (int x2 = x + 1; x2 < width - 1; x2++)
                    {
                        node.AddChild(rowNodes[x2]);
                    }

                    node.AddChild(nextRowNodes[x]);
                }
            }

            return gridGraph[0][0];
        }
    }
}
