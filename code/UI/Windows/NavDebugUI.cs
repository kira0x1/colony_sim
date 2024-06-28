namespace Kira.UI;

using Sandbox.UI;
using Util;

public class NavDebugUI : BaseNavWindow
{
    public Panel MapContainer { get; protected set; }
    private readonly GridTexture gridTexture;
    private readonly Image img;

    public NavDebugUI()
    {
        StyleSheet.Load("UI/Windows/NavDebugUI.cs.scss");
        MapContainer = Add.Panel("map");
        gridTexture = new GridTexture();

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

        Grid grid = new Grid(gridTexture.GridCellsAmount, gridTexture.GridCellsAmount);

        grid.Cells[1, 1].IsOccupied = true;
        grid.Cells[3, 3].IsOccupied = true;

        img.Texture = gridTexture.CreateGridTexture(imgWidth, imgHeight, grid, 1f, false);
    }
}