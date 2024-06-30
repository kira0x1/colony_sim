namespace Kira.UI;

using Sandbox.Razor;
using Sandbox.UI;
using Util;

public class NavDebugUI : BaseNavWindow
{
    public TabContainer ParentTabContainer { get; set; }
    public Panel MapContainer { get; protected set; }
    private readonly Image img;
    private readonly Grid grid;

    public NavDebugUI()
    {
        StyleSheet.Load("UI/Windows/NavDebugUI.cs.scss");
        MapContainer = Add.Panel("map");
        grid = new Grid(21, 2);

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

    /// <summary>
    /// Generate the grid, and map the cells to villagers
    /// </summary>
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
            int x = (cmVillager.PosX - 1 + grid.CellAxis / 2f).CeilToInt();
            int y = (cmVillager.PosY - 1 + grid.CellAxis / 2f).CeilToInt();

            if (x >= 0 && x < grid.CellCount && (y >= 0 && y < grid.CellCount))
            {
                grid.Cells[x, y].IsOccupied = true;
                grid.Cells[x, y].Color = cmVillager.Color;
            }
        }

        img.Texture = grid.CreateGridTexture(imgWidth, imgHeight, false);
    }

    private GridCell cellHovering { get; set; }

    protected override void OnMouseMove(MousePanelEvent e)
    {
        GridCell cell = grid.GetCellByPos(e.LocalPosition.x, e.LocalPosition.y, false);

        if (!cell.Equals(cellHovering))
        {
            if (cellHovering != null)
            {
                cellHovering.Color = Color.Transparent;
            }

            cell.Color = Color.Orange;
            cellHovering = cell;
            grid.GridTexture.Texture.Update(Color.Red, grid.ConvertToViewportCell(cell.X, cell.Y));
        }
    }
}