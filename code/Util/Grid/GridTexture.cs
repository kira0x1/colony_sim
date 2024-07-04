namespace Kira.Util;

using System;
using System.Globalization;

public class GridTexture
{
    private readonly int gridLength;
    private readonly float gridThickness;
    private const float borderThickness = 4f;

    private readonly Color gridColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
    private readonly Color borderColor = Color.White;
    private Grid gridData;

    public Texture Texture { get; private set; }
    public int ImageWidth { get; private set; }
    public int ImageHeight { get; private set; }

    public GridTexture(int cells, int gridThickness = 4)
    {
        gridLength = cells;
        this.gridThickness = gridThickness;
    }

    public Texture CreateGridTexture(int width, int height, Grid grid, bool drawBorders = true)
    {
        this.gridData = grid;
        this.ImageWidth = width;
        this.ImageHeight = height;
        List<byte> gridBytes = CreateGridData(width, height, drawBorders);
        Texture = CreateTexture(gridBytes, width, height);
        return Texture;
    }

    public List<byte> CreateGridData(float width, float height, bool withBorders = true)
    {
        float gapX = width / gridLength;
        float gapY = height / gridLength;

        float halfGridThickness = gridThickness / 2f;
        float innerGridOffset = halfGridThickness * 1.5f;

        List<byte> data = new List<byte>();

        int cellX = 0;
        int cellY = 0;

        const float thicknessOffset = borderThickness / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float rtx = width / gridLength;
                float rty = height / gridLength;

                if (x > rtx * (cellX + 1)) cellX++;
                if (y > rty * (cellY + 1)) cellY++;

                GridCell cell = gridData.Cells[cellX, cellY];

                // Grid Checks
                bool innerGridX = Math.Abs((x + halfGridThickness) % gapX) < gridThickness;
                bool innerGridY = Math.Abs((y + halfGridThickness) % gapY) < gridThickness;

                // Border Checks
                if (x >= width - innerGridOffset) innerGridX = false; // Right Side
                if (x <= innerGridOffset + thicknessOffset) innerGridX = false; // Left Side
                if (y >= height - innerGridOffset) innerGridY = false;
                if (y < innerGridOffset) innerGridY = false;

                var cellColor = cell.IsOccupied ? cell.Color : Color.Transparent;

                if (innerGridX) cellColor = gridColor;
                if (innerGridY) cellColor = gridColor;

                if (withBorders)
                {
                    if (y < borderThickness - borderThickness / 2f) cellColor = borderColor; // Top Border
                    if (x < borderThickness - borderThickness / 2f) cellColor = borderColor; // Left Border
                    if (y + 1 > height - borderThickness / 2f) cellColor = borderColor; // Bottom Border
                    if (x + 1 > width - borderThickness / 2f) cellColor = borderColor; // Right Border
                }

                byte[] cellBytes = ConvertColorToByte(cellColor);
                data.AddRange(cellBytes);
            }

            cellX = 0;
        }

        return data;
    }


    private static byte[] ConvertColorToByte(Color c)
    {
        var data = new byte[4];

        int r = (c.r * 255).CeilToInt();
        int g = (c.g * 255).CeilToInt();
        int b = (c.b * 255).CeilToInt();
        int a = (c.a * 255).CeilToInt();


        data[0] = byte.Parse(r.ToString(CultureInfo.InvariantCulture));
        data[1] = byte.Parse(g.ToString(CultureInfo.InvariantCulture));
        data[2] = byte.Parse(b.ToString(CultureInfo.InvariantCulture));
        data[3] = byte.Parse(a.ToString(CultureInfo.InvariantCulture));

        return data;
    }

    private static Texture CreateTexture(List<byte> data, int width, int height)
    {
        // Create a compute shader from a .shader file
        var computeShader = new ComputeShader("code/shaders/map_shader");

        // Create a texture for the compute shader to use
        var texture = Texture.Create(width, height)
            .WithUAVBinding() // Needs to have this if we're using it in a compute shader
            .WithFormat(ImageFormat.RGBA8888) // Use whatever you need
            .WithData(data.ToArray())
            .Finish();

        // Attach texture to OutputTexture attribute in shader
        computeShader.Attributes.Set("OutputTexture", texture);

        // Dispatch 
        computeShader.Dispatch(texture.Width, texture.Height, 1);
        return texture;
    }
}