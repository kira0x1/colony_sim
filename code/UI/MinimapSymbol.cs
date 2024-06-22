using Sandbox.UI;

namespace Kira.UI;

public class MinimapSymbol : Panel
{
    public List<VillagerData> villagers = new List<VillagerData>();

    public override void OnLayout(ref Rect layoutRect)
    {
        base.OnLayout(ref layoutRect);

        var parentRect = Parent.Box.Rect;
        Log.Info($"parent: {parentRect.WithoutPosition.Bottom}");
        Log.Info(layoutRect.Position);
    }
}