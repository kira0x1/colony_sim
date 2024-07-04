namespace Kira.Util;

using System;

public class Graph
{
    public List<GraphNode> AllNodes { get; set; }

    public Graph()
    {
        AllNodes = new List<GraphNode>();

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                AllNodes.Add(new GraphNode(x, y));
            }
        }
    }

    public List<Vector2Int> Neighbours(GraphNode node)
    {
        Vector2Int[] dirs = { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (var dir in dirs)
        {
            var nb = new Vector2Int(node.x + dir.x, node.y + dir.y);

            if (AllNodes.Exists(n => n.Equals(nb)))
            {
                neighbors.Add(nb);
            }
        }

        return neighbors;
    }

    public GraphNode FindNode(Vector2Int pos)
    {
        GraphNode node = AllNodes.Find(n => n.Equals(pos));
        return node;
    }
}

public class GraphNode : IEquatable<Vector2Int>
{
    public int x;
    public int y;

    public GraphNode(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(Vector2Int other)
    {
        return other.x == x && other.y == y;
    }
}