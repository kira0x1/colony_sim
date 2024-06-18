using System;
using Sandbox.Utility;

namespace Kira;

public sealed partial class MiniMap
{
    [Group("Generator"), Property, Range(0f, 3f)]
    public float Scale { get; set; } = 1f;

    [Group("Generator"), Property, Range(1f, 5f)]
    public float ZoomIn { get; set; } = 1f;

    [Group("Generator"), Property, Range(1f, 10f)]
    public float ZoomOut { get; set; } = 1f;

    [Group("Generator"), Property, Range(0, 3)]
    public float Intensity { get; set; } = 1f;

    [Group("Generator"), Property]
    public bool ClampValues { get; set; } = false;

    [Group("Generator"), Property]
    public NoiseTypes NoiseType { get; set; }

    [Group("Generator"), Property, Range(1, 12), ShowIf(nameof(NoiseType), NoiseTypes.Fbm)]
    public int Octaves { get; set; } = 8;

    [Group("Grid"), Property] public bool DisplayGrid { get; set; } = false;
    [Group("Grid"), Property, Range(0, 10)] public int GridCellsAmount { get; set; } = 5;
    [Group("Grid"), Property] public Color GridColor { get; set; } = Color.White;
    [Group("Grid"), Property, Range(0, 255)] public float GridLuminance { get; set; } = 100f;
    [Group("Grid"), Property, Range(0, 60)] public float GridThickness { get; set; } = 4f;
    [Group("Grid"), Property, Range(0, 60)] public float BorderThickness { get; set; } = 4f;


    public float[,] CreateNoise(int pixelSize = 512)
    {
        float finalZoom = ZoomIn / ZoomOut;
        float finalScale = Scale / finalZoom;

        float[,] data = new float[pixelSize, pixelSize];

        for (int y = 0; y < pixelSize; y++)
        {
            for (int x = 0; x < pixelSize; x++)
            {
                float px = x * finalScale;
                float py = y * finalScale;

                float point = NoiseType switch
                {
                    NoiseTypes.Perlin  => Noise.Perlin(px, py),
                    NoiseTypes.Simplex => Noise.Simplex(px, py),
                    NoiseTypes.Fbm     => Noise.Fbm(Octaves, px, py),
                    _                  => 0f
                };

                float val;

                if (ClampValues)
                {
                    val = MathF.Round(point * Intensity) * 255f;
                }
                else
                {
                    val = point * (Intensity * 255f);
                }

                // val = ClampValues ? MathF.Floor(point * Intensity + 200) : point * 255f;
                data[y, x] = val;
            }
        }

        return data;
    }

    public List<byte> CreateGridData()
    {
        float gridGap = SpriteSize;
        float canvasSize = SpriteSize;

        float gridCellsF = GridCellsAmount;
        float halfGT = GridThickness / 2f;
        float halfGCl = gridCellsF / 2f;
        float halfCanvas = canvasSize / 2f;

        if (GridCellsAmount > 0 && SpriteSize > 0)
        {
            gridGap = canvasSize / GridCellsAmount;
        }


        List<byte> data = new List<byte>();


        for (int y = 0; y < SpriteSize; y++)
        {
            for (int x = 0; x < SpriteSize; x++)
            {
                float gmx = halfGT * 1.5f;

                bool gx = Math.Abs((x + halfGT) % gridGap) < GridThickness;

                if (x >= SpriteSize - gmx) gx = false;
                if (x <= gmx) gx = false;


                bool gy = Math.Abs((y + halfGT) % gridGap) < GridThickness;
                if (y >= SpriteSize - gmx) gy = false;
                if (y <= gmx) gy = false;

                var cellColor = Color.Transparent;


                if (y < BorderThickness - 1 || y >= SpriteSize - BorderThickness)
                {
                    cellColor = GridColor;
                }
                else if (gx)
                {
                    cellColor = GridColor;
                }


                if (x >= SpriteSize - BorderThickness || x < BorderThickness - 1)
                {
                    cellColor = GridColor;
                }
                else if (gy)
                {
                    cellColor = GridColor;
                }

                byte[] cellBytes = ConvertColorToByte(cellColor);

                data.AddRange(cellBytes);
            }
        }

        return data;
    }
}