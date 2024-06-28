namespace Kira.Util;

public class Grid
{
    public GridCell[,] Cells { get; set; }

    public Grid(int cellsX, int cellsY)
    {
        Cells = new GridCell[cellsX, cellsY];

        for (var x = 0; x < Cells.GetLength(0); x++)
        {
            for (var y = 0; y < Cells.GetLength(1); y++)
            {
                GridCell gridCell = new GridCell(x, y);
                Cells[x, y] = gridCell;
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