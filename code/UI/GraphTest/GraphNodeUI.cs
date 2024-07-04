namespace Kira.UI;

using Sandbox.UI;
using Sandbox.UI.Construct;
using Util;

public class GraphNodeUI : Panel
{
    private GraphNode node;
    private List<GraphNode> neighbours;

    public GraphNodeUI(GraphNode node, Graph graph)
    {
        this.node = node;
        if (node != null)
        {
            neighbours = graph.Neighbours(node);

            if (neighbours.Count == 0)
                Add.Label(node.name);
            else
            {
                foreach (GraphNode nb in neighbours)
                {
                    Add.Label($"{node.name} -> {nb.name}");
                }
            }

            Add.Label($"{node.x}, {node.y}");
        }
    }

    public override void OnHotloaded()
    {
        base.OnHotloaded();
        Refresh();
    }

    public override void FinalLayout(Vector2 offset)
    {
        base.FinalLayout(offset);
        Refresh();
    }

    private void Refresh()
    {
        const int gap = 4;
        const float offset = 0f;

        var rect = Box.Rect;
        var parent = Parent.Box;

        var height = rect.Height + gap;
        var width = rect.Width + gap;

        var posx = node.x * width;
        var posy = node.y * height;

        posx += parent.Rect.Position.x + offset;
        posy += parent.Rect.Position.y + offset;

        rect.Position = new Vector2(posx, posy);

        Box.Rect = rect;

        FinalLayoutChildren(Box.Rect.Position);
    }

    public override void OnLayout(ref Rect layoutRect)
    {
        base.OnLayout(ref layoutRect);
        Refresh();
    }
}