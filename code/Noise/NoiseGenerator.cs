using System;
using Sandbox.Utility;
using Component = Sandbox.Component;

namespace Kira;

public sealed class NoiseGenerator : Component, Component.ExecuteInEditor
{
    [Property, MinMax(0, 20), Range(0f, 25f)] public float Scale { get; set; } = 10f;
    [Property, MinMax(0, 10), Range(0, 14)] public float Intensity { get; set; } = 2f;

    [Property] public bool ClampValues { get; set; } = false;
    [Property, ShowIf(nameof(NoiseType), NoiseTypes.Fbm)] public int Octaves { get; set; } = 8;
    [Property] public NoiseTypes NoiseType { get; set; }
    [Property] public bool UseRenderer { get; set; }

    public NoiseGenerator()
    {
    }

    public NoiseGenerator(bool useRenderer)
    {
        UseRenderer = useRenderer;
    }

    private INoiseRenderer Renderer { get; set; }

    public enum NoiseTypes
    {
        Perlin,
        Simplex,
        Fbm
    }

    public float[,] Luminance { get; private set; }

    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateNoise();
    }

    public void UpdateNoise()
    {
        if (UseRenderer)
        {
            Renderer = Components.Get<INoiseRenderer>();
            if (Renderer.Enabled == false) return;
            CreateNoise(Renderer.SpriteSize);
            Renderer.CreateTexture(Luminance);
        }
    }

    public float[,] CreateNoise(float scale, int size = 10)
    {
        Scale = scale;
        return CreateNoise(size);
    }

    public float[,] CreateNoise(int pixelSize = 512)
    {
        Luminance = new float[pixelSize, pixelSize];

        for (int y = 0; y < pixelSize; y++)
        {
            for (int x = 0; x < pixelSize; x++)
            {
                float px = x * Scale;
                float py = y * Scale;


                float point = NoiseType switch
                {
                    NoiseTypes.Perlin  => Noise.Perlin(px, py),
                    NoiseTypes.Simplex => Noise.Simplex(px, py),
                    NoiseTypes.Fbm     => Noise.Fbm(Octaves, px, py),
                    _                  => 0f
                };


                float val = ClampValues ? MathF.Floor(point * Intensity) : point * Intensity;
                Luminance[y, x] = val;
            }
        }

        return Luminance;
    }
}