namespace Kira.UI;

using Sandbox.Razor;
using Sandbox.UI;
using Util;

public class NavDebugUI : BaseNavWindow
{
    public TabContainer ParentTabContainer { get; set; }
    public Panel MapContainer { get; protected set; }
    private readonly Image img;
    private Grid grid;

    public NavDebugUI()
    {
        StyleSheet.Load("UI/Windows/NavDebugUI.cs.scss");
        MapContainer = Add.Panel("map");
        grid = new Grid(7);
        // grid.Cells[0, 0].IsOccupied = true;

        img = new Image();
        img.AddClass("grid");
        MapContainer.AddChild(out img);
    }

    public override void OnHotloaded()
    {
        base.OnHotloaded();
        FinalLayoutChildren(this.Box.Rect.Position);
    }

    protected override void FinalLayoutChildren(Vector2 offset)
    {
        base.FinalLayoutChildren(offset);
        Init();
    }

    private void Init()
    {
        Rect imgRect = img.Box.Rect;
        float width = this.Box.Rect.Width;
        float height = this.Box.Rect.Height;
        imgRect.Width = width;
        imgRect.Height = height;
        img.Box.Rect = imgRect;

        int imgWidth = imgRect.Width.CeilToInt();
        int imgHeight = imgRect.Height.CeilToInt();

        var cm = ColonyManager.Instance;
        grid.ResetCells();

        foreach (Villager cmVillager in cm.Villagers)
        {
            int x = ((cmVillager.PosX - 1) + grid.CellAxis / 2f).CeilToInt();
            int y = ((cmVillager.PosY - 1) + grid.CellAxis / 2f).CeilToInt();

            if (x >= 0 && x < grid.CellCount && (y >= 0 && y < grid.CellCount))
            {
                grid.Cells[x, y].IsOccupied = true;
                grid.Cells[x, y].Color = cmVillager.Color;
            }

            // we get by viewport so 0,0 should be in the center basically gridcells / 2, gridcells / 2, and everything before that should be negative
            // if we had 10x10 grid 0,0 would be 10/2,10/2 = (5,5)
            // but to the array [0,0] the top left corner would be (-5,-5)
            // so to convert [5,5] into (0,0) we  have to do viewport = (0,0), array = (viewport + 10/2) = 5,5
            // so (1,1) would be (1 + 5 = 6) (6,6)
            // and to reverse the conversion it would be
            // (x - 5) so 0,0 i.e the center would be (0 - 5) = -5
            // this doesnt factor in resolution

            //TODO: factor in resolution
        }

        // grid.Cells[0, 0].IsOccupied = true;
        // grid.Cells[3, 1].IsOccupied = true;
        // grid.Cells[1, 1].IsOccupied = true;

        img.Texture = grid.CreateGridTexture(imgWidth, imgHeight, false);
    }

    protected override void OnAfterTreeRender(bool firstTime)
    {
        if (!firstTime)
            Log.Info("is active");
    }

    protected override void BuildRenderTree(RenderTreeBuilder tree)
    {
        base.BuildRenderTree(tree);
        Log.Info($"isactive: {IsActive}");
    }

    protected override int BuildHash()
    {
        return System.HashCode.Combine(false);
    }
}