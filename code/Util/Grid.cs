namespace Kira.Util;

public class Grid
{
    public GridCell[,] Cells { get; set; }
    public GridTexture GridTexture { get; set; }
    public int BaseCellCount { get; internal set; }
    public int CellAxis { get; set; }
    public int CellCount { get; internal set; }

    public Grid(int cells = 5, int borderThickness = 1)
    {
        BaseCellCount = cells;

        CellAxis = cells;
        CellCount = CellAxis * CellAxis;
        // populate cells
        CreateCells();

        // generate texture
        GridTexture = new GridTexture(cells, borderThickness);
    }

    /// <summary>
    /// Populates grid cells
    /// </summary>
    private void CreateCells()
    {
        Cells = new GridCell[CellCount, CellCount];

        for (var x = 0; x < Cells.GetLength(0); x++)
        {
            for (var y = 0; y < Cells.GetLength(1); y++)
            {
                GridCell gridCell = new GridCell(x, y);
                Cells[x, y] = gridCell;
            }
        }
    }

    /// <summary>
    /// Generates the texture
    /// </summary>
    /// <param name="imgWidth"></param>
    /// <param name="imgHeight"></param>
    /// <param name="hasBorders"></param>
    /// <returns></returns>
    public Texture CreateGridTexture(int imgWidth, int imgHeight, bool hasBorders)
    {
        return GridTexture.CreateGridTexture(imgWidth, imgHeight, this, hasBorders);
    }

    /// <summary>
    /// Sets all GridCells to Not Occupied
    /// </summary>
    public void ResetCells()
    {
        for (var x = 0; x < Cells.GetLength(0); x++)
        {
            for (var y = 0; y < Cells.GetLength(1); y++)
            {
                Cells[x, y].IsOccupied = false;
            }
        }
    }
}

public class GridCell
{
    public int X { get; internal set; }
    public int Y { get; internal set; }
    public bool IsOccupied { get; internal set; }
    public Color Color { get; internal set; }

    public GridCell(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }

    public override string ToString()
    {
        return $"x: {X}, y: {Y}";
    }
}