using System;
using Sandbox.Utility;

namespace Kira;

public enum NoiseTypes
{
    Perlin,
    Simplex,
    Fbm
}

public partial class MiniMap
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

    public float[,] CreateNoise(int pixelSize = 512)
    {
        float finalZoom = ZoomIn / ZoomOut;
        float finalScale = Scale / finalZoom;

        var lum = new float[pixelSize, pixelSize];

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
                    val = MathF.Round(point) * Intensity * 255f;
                }
                else
                {
                    val = point * Intensity * 255f;
                }

                // val = ClampValues ? MathF.Floor(point * Intensity + 200) : point * 255f;
                lum[y, x] = val;
            }
        }

        return lum;
    }
}