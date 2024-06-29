namespace Kira.Util;

using System;

public class Grid
{
    public GridCell[,] Cells { get; set; }
    public GridTexture GridTexture { get; set; }
    public int Resolution { get; internal set; }
    public int BaseCellCount { get; internal set; }
    public int CellAxis { get; set; }
    public int CellCount { get; internal set; }

    private int imageWidth;
    private int imageHeight;

    public Grid(int cells = 5, int resolution = 1)
    {
        Resolution = resolution;
        BaseCellCount = cells;
        CellCount = cells * cells * resolution;
        CellAxis = cells * resolution;
        CellCount = CellAxis * CellAxis;

        Resolution = resolution;
        CreateCells();
        GridTexture = new GridTexture(cells, resolution);
    }

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

    public Texture CreateGridTexture(int imgWidth, int imgHeight, bool hasBorders)
    {
        imageWidth = imgWidth;
        imageHeight = imgHeight;
        return GridTexture.CreateGridTexture(imgWidth, imgHeight, this, Resolution, hasBorders);
    }

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

    public GridCell GetCellByPos(float mx, float my, out Vector2Int CellIndex)
    {
        var xCell = ((mx + 1) / imageWidth * CellAxis).FloorToInt();
        var yCell = ((my + 1) / imageHeight * CellAxis).FloorToInt();

        xCell = xCell.Clamp(0, BaseCellCount - 1);
        yCell = yCell.Clamp(0, BaseCellCount - 1);

        GridCell cell = Cells[xCell, yCell];
        CellIndex.x = xCell;
        CellIndex.y = yCell;
        return cell;
    }

    public Vector2Int ConvertToGridPosition(float x, float y)
    {
        var xCell = ((x + 1) / imageWidth * CellAxis).FloorToInt();
        var yCell = ((y + 1) / imageHeight * CellAxis).FloorToInt();
        xCell = xCell.Clamp(0, BaseCellCount - 1);
        yCell = yCell.Clamp(0, BaseCellCount - 1);

        return new Vector2Int(xCell, yCell);
    }

    public Texture UpdateTextureSection(float x, float y)
    {
        var pos = ConvertToGridPosition(x, y);
        Log.Info($"POS: {pos}");
        int ratio = imageWidth / CellCount;

        int xMin = ratio * pos.x;
        int xMax = ratio * (pos.x + 1);

        int yMin = ratio * pos.y;
        int yMax = ratio * (pos.y + 1);

        var tx = GridTexture.UpdateTextureSection(xMin, xMax, yMin, yMax);
        return tx;
    }

    public Texture UpdateTextureSection(int minX, int maxX, int minY, int maxY)
    {
        var tx = GridTexture.UpdateTextureSection(minX, maxX, minY, maxY);
        return tx;
    }

    public GridCell GetCellByPos(float mx, float my, bool withLog = false)
    {
        if (withLog)
        {
            Log.Info($"imgWidth: {imageWidth}");
        }

        var pos = ConvertToGridPosition(mx, my);

        int ratio = imageWidth / CellAxis;
        int xMin = ratio * pos.x;
        int xMax = ratio * (pos.x + 1);

        if (withLog)
        {
            Log.Info("");
            Log.Info($"xmin: {xMin}");
            Log.Info($"xmax: {xMax}");
            Log.Info(pos);
            Log.Info("");
        }

        GridCell cell = Cells[pos.x, pos.y];
        return cell;
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