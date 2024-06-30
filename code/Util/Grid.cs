namespace Kira.Util;

using System;

public class Grid
{
    public GridCell[,] Cells { get; set; }
    public GridTexture GridTexture { get; set; }
    public int BaseCellCount { get; internal set; }
    public int CellAxis { get; set; }
    public int CellCount { get; internal set; }
    private int borderThickness { get; set; }

    public Grid(int cells = 5, int borderThickness = 1)
    {
        this.borderThickness = borderThickness;
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


    public GridCell GetCellByPos(float mx, float my, bool withLog = false)
    {
        var pos = ConvertToGridPosition(mx, my);
        GridCell cell = Cells[pos.x, pos.y];
        return cell;
    }

    public Rect ConvertToViewportCell(int x, int y)
    {
        int imageWidth = GridTexture.ImageWidth;

        int ratio = imageWidth / CellAxis;


        int xMin = ratio * x;
        int xMax = ratio * (x + 1);
        int yMin = ratio * y;
        int yMax = ratio * (y + 1);

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    public Vector2Int ConvertToGridPosition(float x, float y)
    {
        int imageWidth = GridTexture.ImageWidth;
        int imageHeight = GridTexture.ImageHeight;

        var xCell = (x / imageWidth * CellAxis).FloorToInt();
        var yCell = (y / imageHeight * CellAxis).FloorToInt();

        xCell = xCell.Clamp(0, BaseCellCount);
        yCell = yCell.Clamp(0, BaseCellCount);
        return new Vector2Int(xCell, yCell);
    }
}

public class GridCell : IEqualityComparer<GridCell>, IEqualityComparer<Vector2Int>
{
    public int X { get; internal set; }
    public int Y { get; internal set; }
    public bool IsOccupied { get; internal set; }
    public Color Color { get; internal set; } = Color.Transparent;

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

    public bool Equals(GridCell cell1, GridCell cell2)
    {
        if (ReferenceEquals(cell1, cell2)) return true;
        if (ReferenceEquals(cell1, null)) return false;
        if (ReferenceEquals(cell2, null)) return false;
        if (cell1.GetType() != cell2.GetType()) return false;
        return cell1.X == cell2.X && cell1.Y == cell2.Y;
    }


    public bool Equals(Vector2Int cell1, Vector2Int cell2)
    {
        return cell1.x == cell2.x && cell1.y == cell2.y;
    }

    public int GetHashCode(GridCell obj)
    {
        return HashCode.Combine(obj.X, obj.Y);
    }

    public int GetHashCode(Vector2Int obj)
    {
        return HashCode.Combine(obj.x, obj.y);
    }
}