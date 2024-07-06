namespace Kira.Util;

using System;
using System.Threading.Tasks;

public class Graph
{
    public List<GraphNode> AllNodes { get; set; }
    public List<GraphNode> RealNodes { get; set; }
    private readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public bool IsSearching { get; private set; }

    public Graph()
    {
        AllNodes = new List<GraphNode>();

        const int ylength = 5;
        const int xlength = 5;

        int i = 0;
        int j = 0;

        for (int x = 0; x < xlength; x++)
        {
            for (int y = 0; y < ylength; y++)
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


        // Set the center slot as occupied
        AllNodes[ylength * xlength / 2].IsOccupied = true;
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
        var reached = new List<GraphNode>();
        var frontier = new Queue<GraphNode>();

        frontier.Enqueue(start);
        reached.Add(start);

        RealTimeSince timeSinceStart = 0;

        GraphNode previousCurrent = null;
        List<GraphNode> prevNeighbours = new List<GraphNode>();

        while (frontier.Count > 0)
        {
            if (timeSinceStart >= 60)
            {
                Log.Info($"exiting loop");
                break;
            }

            var current = frontier.Dequeue();
            current.IsCurrent = true;

            if (previousCurrent != null)
            {
                previousCurrent.IsCurrent = false;
            }

            await Task.Delay(150);
            foreach (GraphNode prevNeighbour in prevNeighbours)
            {
                prevNeighbour.IsNeighbour = false;
            }

            previousCurrent = current;

            var neighbours = Neighbours(current);

            foreach (GraphNode nb in neighbours)
            {
                nb.IsNeighbour = true;
                await Task.Delay(15);

                if (!reached.Contains(nb))
                {
                    frontier.Enqueue(nb);
                    nb.IsReached = true;
                    reached.Add(nb);
                }
            }

            prevNeighbours = neighbours;
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

    public bool IsOccupied { get; set; }

    // Is not just apart of the grid / for aesthetic purposes
    public bool IsRealNode { get; set; }

    // Just for displaying a neighbour during a search
    public bool IsNeighbour { get; set; }

    public bool IsReached { get; set; }

    public bool IsWall { get; set; }

    // is the node currently selected for searching
    public bool IsCurrent { get; set; }

    public GraphNode(int x, int y, string name = "", bool isRealNode = false, bool isWall = false)
    {
        this.x = x;
        this.y = y;
        this.name = name;
        this.IsRealNode = isRealNode;
        this.IsWall = isWall;
    }

    public bool Equals(Vector2Int other)
    {
        return other.x == x && other.y == y;
    }
}