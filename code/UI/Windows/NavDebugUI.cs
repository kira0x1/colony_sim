namespace Kira.UI;

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
        // Load stylesheet
        StyleSheet.Load("UI/Windows/NavDebugUI.cs.scss");

        MapContainer = Add.Panel("map");

        // Create grid overlay and add it to the mapcontainer
        grid = new Grid(21, 2);
        img = new Image();
        img.AddClass("grid");

        MapContainer.AddChild(out img);

        // Finally generate the texture
        GenerateTexture();
    }

    public void UpdatePanel()
    {
        GenerateTexture();
    }

    /// <summary>
    /// Generate the grid, and map the cells to villagers
    /// </summary>
    private void GenerateTexture()
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
            float x = cmVillager.PosX;
            float y = cmVillager.PosY;

            if (GetGridCell(x, y, out GridCell cl))
            {
                cl.IsOccupied = true;
                cl.Color = cmVillager.Color;
            }
        }

        img.Texture = grid.CreateGridTexture(imgWidth, imgHeight, false);
    }

    private bool GetGridCell(float x, float y, out GridCell cell)
    {
        int posX = (x - 1 + grid.CellAxis / 2f).CeilToInt();
        int posY = (y - 1 + grid.CellAxis / 2f).CeilToInt();

        if (posX >= 0 && posX < grid.CellCount && (posY >= 0 && posX < grid.CellCount))
        {
            cell = grid.Cells[posX, posY];
            return true;
        }

        cell = null;
        return false;
    }

    // Update UI on hotload
    public override void OnHotloaded()
    {
        base.OnHotloaded();
        FinalLayoutChildren(this.Box.Rect.Position);
    }

    // Regenerate texture if layout has changed
    protected override void FinalLayoutChildren(Vector2 offset)
    {
        base.FinalLayoutChildren(offset);
        GenerateTexture();
    }
}