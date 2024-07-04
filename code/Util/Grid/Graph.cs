namespace Kira.Util;

using System;

public class Graph
{
    public List<GraphNode> AllNodes { get; set; }
    private char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public Graph()
    {
        AllNodes = new List<GraphNode>();

        const int ylength = 6;
        const int xlength = 5;

        for (int x = 0; x < xlength; x += 2)
        {
            for (int y = 0; y < ylength; y += 2)
            {
                int i = x * 5 + y;
                Vector2Int pos = new Vector2Int(x, y);

                if (i > 1)
                {
                    int xRand = Random.Shared.Int(0, 2);
                    int yRand = Random.Shared.Int(0, 1);

                    pos.x += xRand;
                    pos.y += yRand;
                }

                AllNodes.Add(new GraphNode(pos.x, pos.y, Letters[i].ToString()));
            }
        }
    }

    public List<GraphNode> Neighbours(GraphNode node)
    {
        Vector2Int[] dirs =
            { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };

        List<GraphNode> neighbors = new List<GraphNode>();

        foreach (var dir in dirs)
        {
            // neighbour position
            var npos = new Vector2Int(node.x + dir.x, node.y + dir.y);

            // neighbour node
            var nb = AllNodes.Find(x => x.Equals(npos));

            if (nb != null)
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
    public string name;
    public int x;
    public int y;

    public GraphNode(int x, int y, string name = "")
    {
        this.x = x;
        this.y = y;
        this.name = name;
    }

    public bool Equals(Vector2Int other)
    {
        return other.x == x && other.y == y;
    }
}