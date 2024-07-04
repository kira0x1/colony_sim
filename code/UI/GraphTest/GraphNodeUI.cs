namespace Kira.UI;

using Sandbox.UI;
using Sandbox.UI.Construct;
using Util;

public class GraphNodeUI : Panel
{
    private GraphNode node;

    public GraphNodeUI(GraphNode node)
    {
        this.node = node;
        if (node != null)
        {
            this.Add.Label($"{node.x}, {node.y}");
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
        const int gap = 15;

        var rect = Box.Rect;
        var parent = Parent.Box;

        var height = rect.Height + gap;
        var width = rect.Width + gap;

        var offset = parent.Rect.Position;

        var posx = node.x * width;
        var posy = node.y * height;

        posx += offset.x;
        posy += offset.y;

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