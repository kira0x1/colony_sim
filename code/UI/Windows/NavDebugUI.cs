namespace Kira.UI;

using Sandbox.UI;
using Util;

public class NavDebugUI : BaseNavWindow
{
    public Panel MapContainer { get; protected set; }
    private readonly Image img;
    private Grid grid;

    public NavDebugUI()
    {
        StyleSheet.Load("UI/Windows/NavDebugUI.cs.scss");
        MapContainer = Add.Panel("map");
        grid = new Grid(5, 1);
        grid.Cells[0, 0].IsOccupied = true;

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

        grid.ResetCells();
        grid.Cells[0, 0].IsOccupied = true;
        grid.Cells[3, 1].IsOccupied = true;
        grid.Cells[1, 2].IsOccupied = true;
        img.Texture = grid.CreateGridTexture(imgWidth, imgHeight, false);
    }
}