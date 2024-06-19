using System;
using Sandbox.Utility;

namespace Kira;

public sealed partial class MiniMap : Component, Component.ExecuteInEditor, IHotloadManaged
{
    [Group("Generator"), Property, Range(0f, 3f)] public float Scale { get; set; } = 1f;
    [Group("Generator"), Property, Range(1f, 5f)] public float ZoomIn { get; set; } = 1f;
    [Group("Generator"), Property, Range(1f, 10f)] public float ZoomOut { get; set; } = 1f;
    [Group("Generator"), Property, Range(0, 3)] public float Intensity { get; set; } = 1f;
    [Group("Generator"), Property] public bool ClampValues { get; set; } = false;
    [Group("Generator"), Property] public NoiseTypes NoiseType { get; set; }

    [Group("Generator"), Property, Range(1, 12), ShowIf(nameof(NoiseType), NoiseTypes.Fbm)]
    public int Octaves { get; set; } = 8;

    [Group("Grid"), Property] public bool DisplayGrid { get; set; } = false;
    [Group("Grid"), Property, Range(0, 10)] public int GridCellsAmount { get; set; } = 5;
    [Group("Grid"), Property] public Color GridColor { get; set; } = Color.White;
    [Group("Grid"), Property, Range(0, 255)] public float GridLuminance { get; set; } = 100f;
    [Group("Grid"), Property, Range(0, 60)] public float GridThickness { get; set; } = 4f;
    [Group("Grid"), Property, Range(0, 60)] public float BorderThickness { get; set; } = 4f;
    [Group("Grid"), Property] public Color BorderColor { get; set; } = Color.White;


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

    public List<byte> OverlayGridData(List<byte> data)
    {
        float gridGap = SpriteSize;
        float canvasSize = SpriteSize;

        float halfGridThickness = GridThickness / 2f;
        float innerGridOffset = halfGridThickness * 1.5f;

        if (GridCellsAmount > 0 && SpriteSize > 0)
        {
            gridGap = canvasSize / GridCellsAmount;
        }

        int i = 0;
        for (int y = 0; y < data.Count; y++)
        {
            for (int x = 0; x < data.Count; x++)
            {
                float thicknessOffset = BorderThickness / 2f;
                bool innerGridX = Math.Abs((x + halfGridThickness) % gridGap) < GridThickness;

                // Right Side
                if (x >= SpriteSize - innerGridOffset) innerGridX = false;

                // Left Side
                if (x <= innerGridOffset + thicknessOffset) innerGridX = false;

                bool innerGridY = Math.Abs((y + halfGridThickness) % gridGap) < GridThickness;

                if (y >= SpriteSize - innerGridOffset) innerGridY = false;
                if (y < innerGridOffset) innerGridY = false;

                bool isTopBorder = y < BorderThickness - BorderThickness / 2f;
                bool isRightBorder = x + 1 > SpriteSize - BorderThickness / 2f;
                bool isBottomBorder = y + 1 > SpriteSize - BorderThickness / 2f;
                bool isLeftBorder = x < BorderThickness - BorderThickness / 2f;

                bool isBorder = isTopBorder || isRightBorder || isBottomBorder || isLeftBorder;
                bool isInner = innerGridX || innerGridY;

                if (!isBorder && !isInner) continue;
                var cellColor = isInner ? GridColor : BorderColor;

                byte[] cellBytes = ConvertColorToByte(cellColor);

                data[i] = cellBytes[0];
                data[i + 1] = cellBytes[1];
                data[i + 2] = cellBytes[2];
                data[i + 3] = cellBytes[3];
            }
        }

        return data;
    }

    public List<byte> CreateGridData()
    {
        float gridGap = SpriteSize;
        float canvasSize = SpriteSize;

        float halfGridThickness = GridThickness / 2f;
        float innerGridOffset = halfGridThickness * 1.5f;

        if (GridCellsAmount > 0 && SpriteSize > 0)
        {
            gridGap = canvasSize / GridCellsAmount;
        }

        List<byte> data = new List<byte>();

        for (int y = 0; y < SpriteSize; y++)
        {
            for (int x = 0; x < SpriteSize; x++)
            {
                float thicknessOffset = BorderThickness / 2f;
                bool innerGridX = Math.Abs((x + halfGridThickness) % gridGap) < GridThickness;

                // Right Side
                if (x >= SpriteSize - innerGridOffset) innerGridX = false;

                // Left Side
                if (x <= innerGridOffset + thicknessOffset) innerGridX = false;


                bool innerGridY = Math.Abs((y + halfGridThickness) % gridGap) < GridThickness;

                if (y >= SpriteSize - innerGridOffset) innerGridY = false;
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

                // Top Border
                if (y < BorderThickness - BorderThickness / 2f)
                {
                    cellColor = BorderColor;
                }

                // Right Border
                if (x + 1 > SpriteSize - BorderThickness / 2f)
                {
                    cellColor = BorderColor;
                }

                // Bottom Border
                if (y + 1 > SpriteSize - BorderThickness / 2f)
                {
                    cellColor = BorderColor;
                }

                // Left Border
                if (x < BorderThickness - BorderThickness / 2f)
                {
                    cellColor = BorderColor;
                }

                byte[] cellBytes = ConvertColorToByte(cellColor);
                data.AddRange(cellBytes);
            }
        }

        return data;
    }
}