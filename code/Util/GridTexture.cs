namespace Kira.Util;

using System;
using System.Globalization;

public class GridTexture
{
    [Group("Grid"), Property, Range(0, 10)] public int GridCellsAmount { get; set; } = 5;
    [Group("Grid"), Property, Range(0, 255)] public float GridLuminance { get; set; } = 100f;
    [Group("Grid"), Property, Range(0, 60)] public float GridThickness { get; set; } = 4f;
    [Group("Grid"), Property, Range(0, 60)] public float BorderThickness { get; set; } = 4f;
    [Group("Grid"), Property] public Color GridColor { get; set; } = Color.White;
    [Group("Grid"), Property] public Color BorderColor { get; set; } = Color.White;


    public Texture CreateGridTexture(int width, int height, bool drawBorders = true)
    {
        List<byte> gridData = CreateGridData(width, height, drawBorders);
        return CreateTexture(gridData, width, height);
    }

    public List<byte> CreateGridData(float width, float height, bool withBorders = true)
    {
        float gapX = width / GridCellsAmount;
        float gapY = height / GridCellsAmount;

        float halfGridThickness = GridThickness / 2f;
        float innerGridOffset = halfGridThickness * 1.5f;

        // if (GridCellsAmount > 0 && width > 0 && height > 0)
        // {
        //     gapX = width / GridCellsAmount;
        //     gapY = height / GridCellsAmount;
        // }

        List<byte> data = new List<byte>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float thicknessOffset = BorderThickness / 2f;
                bool innerGridX = Math.Abs((x + halfGridThickness) % gapX) < GridThickness;

                // Right Side
                if (x >= width - innerGridOffset) innerGridX = false;

                // Left Side
                if (x <= innerGridOffset + thicknessOffset) innerGridX = false;


                bool innerGridY = Math.Abs((y + halfGridThickness) % gapY) < GridThickness;

                if (y >= height - innerGridOffset) innerGridY = false;
                if (y < innerGridOffset) innerGridY = false;

                var cellColor = Color.Transparent;

                if (innerGridX)
                {
                    cellColor = GridColor;
                }

                if (innerGridY)
                {
                    cellColor = GridColor;
                }

                if (withBorders)
                {
                    // Top Border
                    if (y < BorderThickness - BorderThickness / 2f)
                    {
                        cellColor = BorderColor;
                    }

                    // Right Border
                    if (x + 1 > width - BorderThickness / 2f)
                    {
                        cellColor = BorderColor;
                    }

                    // Bottom Border
                    if (y + 1 > height - BorderThickness / 2f)
                    {
                        cellColor = BorderColor;
                    }

                    // Left Border
                    if (x < BorderThickness - BorderThickness / 2f)
                    {
                        cellColor = BorderColor;
                    }
                }

                byte[] cellBytes = ConvertColorToByte(cellColor);
                data.AddRange(cellBytes);
            }
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

    private Texture CreateTexture(List<byte> data, int width, int height)
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