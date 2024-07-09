namespace Kira.Util;

using System;
using System.Threading.Tasks;

public class Graph
{
    public int GridRows { get; set; }
    public int GridCols { get; set; }
    public List<GraphNode> AllNodes { get; set; }
    public List<GraphNode> RealNodes { get; set; }

    private readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    // Node Dictionary key = Node, value = the previous node in the path
    private Dictionary<GraphNode, GraphNode> CameFrom { get; set; }
    public bool IsSearching { get; private set; }
    public GraphNode StartNode { get; }
    public GraphNode GoalNode { get; }
    public int SearchDelay { get; set; } = 80;

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

        // Set Wall's
        AllNodes[(GridRows - 2) / 2 + GridCols].IsWall = true;
        AllNodes[(GridRows - 1) / 2 + GridCols].IsWall = true;
        AllNodes[(GridRows + 1) / 2 + GridCols].IsWall = true;
        AllNodes[GridRows - 1 + GridCols + 1].IsWall = true;
        AllNodes[GridRows].IsWall = true;

        // Set Goal node
        AllNodes[0].IsGoal = true;
        GoalNode = AllNodes[0];

        // Set the center slot as occupied
        AllNodes[cols * rows / 2].IsOccupied = true;
        StartNode = AllNodes[cols * rows / 2];
    }

    public void CancelSearch()
    {
        IsSearching = false;
    }

    public void ResetNodes()
    {
        foreach (GraphNode node in AllNodes)
        {
            node.IsCurrent = false;
            node.isFrontier = false;
            node.IsNeighbour = false;
            node.IsReached = false;
            node.IsHighlightedPath = false;
            node.DisplayCameFromDirection = false;
        }
    }

    public async Task StartSearch()
    {
        IsSearching = true;

        var start = AllNodes.Find(x => x.IsOccupied);
        CameFrom = new Dictionary<GraphNode, GraphNode>();
        var frontier = new Queue<GraphNode>();

        frontier.Enqueue(start);
        CameFrom.Add(start, null);

        GraphNode previousCurrent = null;
        List<GraphNode> prevNeighbours = new List<GraphNode>();

        do
        {
            // Set new current node
            var current = frontier.Dequeue();
            current.IsCurrent = true;

            // Reset Previous Current
            if (previousCurrent != null)
            {
                previousCurrent.IsCurrent = false;
                previousCurrent.isFrontier = false;
                previousCurrent.IsReached = true;
            }

            // Reset Previous Neighbours
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
                if (nb.IsWall) continue;
                nb.IsNeighbour = true;

                if (!CameFrom.ContainsKey(nb))
                {
                    // nb.IsReached = true;
                    nb.isFrontier = true;
                    nb.CameFrom = current;
                    nb.DisplayCameFromDirection = true;
                    frontier.Enqueue(nb);
                    CameFrom.Add(nb, current);
                }
            }

            prevNeighbours = neighbours;
            await Task.Delay(SearchDelay);
        } while (frontier.Count > 0 && IsSearching);

        foreach (GraphNode prevNeighbour in prevNeighbours)
        {
            prevNeighbour.IsNeighbour = false;
            prevNeighbour.isFrontier = false;
        }

        IsSearching = false;
    }

    public async void FindPathFromGoal()
    {
        await FindPathFromNode(GoalNode);
    }

    public async Task FindPathFromNode(GraphNode node)
    {
        var current = node;
        var path = new Stack<GraphNode>();
        IsSearching = true;

        while (current != StartNode)
        {
            if (current != GoalNode)
            {
                current.IsHighlightedPath = true;
            }

            path.Push(current);
            current = CameFrom[current];
            await Task.Delay(170);
        }

        IsSearching = false;

        // path.Append(StartNode);
        // path.Reverse();
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
}