namespace Kira.Util;

using System;
using System.Threading.Tasks;

public class Graph
{
    public int GridRows { get; set; } = 5;
    public int GridCols { get; set; } = 5;

    public List<GraphNode> AllNodes { get; set; }
    public List<GraphNode> RealNodes { get; set; }
    private readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public bool IsSearching { get; private set; }

    public Graph(int rows = 5, int cols = 5)
    {
        AllNodes = new List<GraphNode>();

        GridRows = rows;
        GridCols = cols;

        int i = 0;
        int j = 0;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                string letter = Letters[i % Letters.Length].ToString();

                if (i >= Letters.Length)
                {
                    letter += $"{j}";
                    j++;
                }

                var xRand = Random.Shared.Int(1, 3);
                var yRand = Random.Shared.Int(1, 3);

                var isRealNode = x % xRand == 0 && y % yRand == 0;
                AllNodes.Add(new GraphNode(pos.x, pos.y, letter, isRealNode));
                i++;
            }
        }

        // AllNodes[(ylength - 2) / 2 + xlength].IsWall = true;
        // AllNodes[(ylength - 1) / 2 + xlength].IsWall = true;
        // AllNodes[(ylength + 1) / 2 + xlength].IsWall = true; 
        // AllNodes[ylength - 1 + xlength + 1].IsWall = true;
        // AllNodes[ylength].IsWall = true;

        AllNodes[0].IsGoal = true;

        // Set the center slot as occupied
        AllNodes[cols * rows / 2].IsOccupied = true;
    }

    public List<GraphNode> Neighbours(GraphNode node)
    {
        Vector2Int[] dirs =
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

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

    public GraphNode FindNode(int x, int y)
    {
        GraphNode node = AllNodes.Find(n => n.Equals(new Vector2Int(x, y)));
        return node;
    }

    public async Task DoSearch()
    {
        IsSearching = true;

        var start = AllNodes.Find(x => x.IsOccupied);
        var cameFrom = new Dictionary<GraphNode, GraphNode>();
        var frontier = new Queue<GraphNode>();

        frontier.Enqueue(start);
        cameFrom.Add(start, null);

        GraphNode previousCurrent = null;
        List<GraphNode> prevNeighbours = new List<GraphNode>();

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            current.IsCurrent = true;

            if (previousCurrent != null)
            {
                previousCurrent.IsCurrent = false;
                previousCurrent.isFrontier = false;
                previousCurrent.IsReached = true;
            }

            foreach (GraphNode prevNeighbour in prevNeighbours)
            {
                prevNeighbour.IsReached = true;
                prevNeighbour.IsNeighbour = false;
                prevNeighbour.isFrontier = false;
            }

            previousCurrent = current;

            var neighbours = Neighbours(current);

            foreach (GraphNode nb in neighbours)
            {
                nb.IsNeighbour = true;

                if (!cameFrom.ContainsKey(nb))
                {
                    // nb.IsReached = true;
                    nb.isFrontier = true;
                    nb.CameFrom = current;
                    frontier.Enqueue(nb);
                    cameFrom.Add(nb, current);
                }
            }

            prevNeighbours = neighbours;
            await Task.Delay(300);
        }

        foreach (GraphNode prevNeighbour in prevNeighbours)
        {
            prevNeighbour.IsNeighbour = false;
            prevNeighbour.isFrontier = false;
        }

        IsSearching = false;
    }

    private void CreateGraphWithGaps()
    {
        AllNodes = new List<GraphNode>();

        const int ylength = 6;
        const int xlength = 5;
        int i = 0;
        for (int x = 0; x < xlength; x += 2)
        {
            for (int y = 0; y < ylength; y += 2)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (i == 1)
                {
                    // just making sure a -> b are always neighbours for testing purposes
                    pos.y--;
                }
                else if (i > 1)
                {
                    int xRand = Random.Shared.Int(0, 1);
                    int yRand = Random.Shared.Int(0, 1);

                    pos.x += xRand;
                    pos.y += yRand;
                }

                AllNodes.Add(new GraphNode(pos.x, pos.y, Letters[i].ToString()));
                i++;
            }
        }
    }
}

public class GraphNode : IEquatable<Vector2Int>
{
    public string name;
    public int x;
    public int y;

    public Vector2Int Position { get; private set; }

    public bool IsOccupied { get; set; }

    // Is not just apart of the grid / for aesthetic purposes
    public bool IsRealNode { get; set; }

    // Just for displaying a neighbour during a search
    public bool IsNeighbour { get; set; }

    public bool IsReached { get; set; }

    public bool IsWall { get; set; }

    public bool isFrontier { get; set; }

    // is the node currently selected for searching
    public bool IsCurrent { get; set; }

    public bool IsGoal { get; set; }

    public GraphNode CameFrom { get; set; }

    public GraphNode(int x, int y, string name = "", bool isRealNode = false, bool isWall = false)
    {
        this.x = x;
        this.y = y;
        this.Position = new Vector2Int(x, y);
        this.name = name;
        this.IsRealNode = isRealNode;
        this.IsWall = isWall;
    }

    public bool Equals(Vector2Int other)
    {
        return other.x == x && other.y == y;
    }
}