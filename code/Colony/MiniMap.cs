using System;
using System.Globalization;

namespace Kira;

public struct ColorInfo
{
    public int x;
    public int y;
    public Color color;

    public ColorInfo(int x, int y, Color color)
    {
        this.x = x;
        this.y = y;
        this.color = color;
    }
}

[Category("Kira")]
public partial class MiniMap : Component, INoiseRenderer, IHotloadManaged
{
    [Property, Group("Base")] public bool DrawOnSprite { get; set; }
    [Property, Group("Base")] public Color grassColor { get; set; }

    [Property, Group("Base")] public bool UseLayerConfig { get; set; }
    [Property, Group("Base")] public bool UseLayers { get; set; } = false;

    [Property, Group("Base"), ShowIf(nameof(UseLayerConfig), false)] public List<LayerInfo> Layers { get; set; } = new();
    [Property, Group("Base"), ShowIf(nameof(UseLayerConfig), true)] public LayerConfig LayerConfig { get; set; }

    [Property, Group("Base"), Range(1, 512)] public int SpriteSize { get; set; } = 512;
    [Property, Group("Base")] private bool LogMinMax { get; set; } = true;

    // Luminance
    [Group("Luminance"), Property, Range(0, 15)] public float Brightness { get; set; } = 255;
    [Group("Luminance"), Property, Range(0, 255)] public int MinLuminance { get; set; } = 100;
    [Group("Luminance"), Property, Range(0, 255)] public int MaxLuminance { get; set; } = 255;

    [Group("Grid"), Property] public bool DisplayGrid { get; set; } = false;
    [Group("Grid"), Property, Range(0, 60)] public float GridGap { get; set; } = 4;
    [Group("Grid"), Property] public Color GridColor { get; set; } = Color.White;
    [Group("Grid"), Property, Range(0, 255)] public float GridLuminance { get; set; } = 100f;

    [Property, Range(0, 15)]
    public float xDiv { get; set; } = 2f;

    [Property, Range(0, 15f)]
    public float yDiv { get; set; } = 1.01f;

    [Property, Range(0f, 5f)]
    public float thickness { get; set; } = 4f;

    private List<LayerInfo> LayersChosen = new List<LayerInfo>();

    private void Refresh()
    {
        if (!DrawOnSprite) return;

        if (SpriteSize > 512) SpriteSize = 512;
        logAmount = 0;
        LayersChosen = UseLayerConfig ? LayerConfig.Layers : Layers;
        var tx = CreateTexture();

        var sp = GameObject.Components.GetOrCreate<SpriteRenderer>();
        sp.Size = SpriteSize;

        if (!sp.IsValid())
        {
            Log.Info("Could not find sprite renderer");
            return;
        }

        sp.Texture = tx;
    }

    public void Created(IReadOnlyDictionary<string, object> state)
    {
        Refresh();
    }

    protected override void OnValidate()
    {
        Refresh();
    }

    private static void AddColorToData(List<byte> data, Color c, float lum, bool isGrid = false)
    {
        Color finalColor;
        ColorHsv hsvColor = c.ToHsv();

        if (isGrid)
        {
            finalColor = c.ToHsv().WithValue(hsvColor.Value * 255f);
        }
        else
        {
            finalColor = c.Lighten(lum);
        }

        float r = finalColor.r.CeilToInt().Clamp(0, 255);
        float g = finalColor.g.CeilToInt().Clamp(0, 255);
        float b = finalColor.b.CeilToInt().Clamp(0, 255);
        float a = (finalColor.a * 255).CeilToInt().Clamp(0, 255);

        byte rByte = byte.Parse(r.ToString(CultureInfo.InvariantCulture));
        byte gByte = byte.Parse(g.ToString(CultureInfo.InvariantCulture));
        byte bByte = byte.Parse(b.ToString(CultureInfo.InvariantCulture));
        byte aByte = byte.Parse(a.ToString(CultureInfo.InvariantCulture));

        data.Add(rByte);
        data.Add(gByte);
        data.Add(bByte);
        data.Add(aByte);
    }

    public Texture CreateTexture()
    {
        if (SpriteSize > 512) SpriteSize = 512;

        CreateNoise(SpriteSize);

        // Create a compute shader from a .shader file
        var computeShader = new ComputeShader("code/shaders/map_shader");

        List<byte> data = GetColorInfo(Luminance);

        // Create a texture for the compute shader to use
        var texture = Texture.Create(SpriteSize, SpriteSize)
            .WithUAVBinding()                 // Needs to have this if we're using it in a compute shader
            .WithFormat(ImageFormat.RGBA8888) // Use whatever you need
            .WithData(data.ToArray())
            .Finish();

        // Attach texture to OutputTexture attribute in shader
        computeShader.Attributes.Set("OutputTexture", texture);

        // Dispatch 
        computeShader.Dispatch(texture.Width, texture.Height, 1);
        return texture;
    }

    private LayerInfo GetLayerByLuminance(float lum)
    {
        LayerInfo layer = LayersChosen.Find(f => f.Start <= lum && f.End >= lum);
        if (layer == null)
        {
            layer = new LayerInfo();
            if (logAmount < 2)
            {
                Log.Info($"layer for {lum} not found");
                logAmount++;
            }
        }

        return layer;
    }

    int logAmount { get; set; } = 0;

    private List<byte> GetColorInfo(float[,] noiseValues)
    {
        List<byte> data = new List<byte>();
        List<float> lvals = new List<float>();

        for (int x = 0; x < SpriteSize; x++)
        {
            for (int y = 0; y < SpriteSize; y++)
            {
                float finalBrightness = Brightness;
                if (!ClampValues) finalBrightness /= 2f;

                float lum = noiseValues[x, y] * finalBrightness;
                lum = lum.CeilToInt().Clamp(MinLuminance, MaxLuminance);
                lvals.Add(lum);
                var baseColor = grassColor;
                bool useGrid = DisplayGrid && (x % GridGap < 1 || y % GridGap < 1);

                if (useGrid)
                {
                    baseColor = GridColor;
                }
                else if (Math.Abs(x - SpriteSize / xDiv) < thickness && Math.Abs(y - SpriteSize / yDiv) < thickness)
                {
                    baseColor = Color.Cyan;
                }
                else if (UseLayers)
                {
                    var layer = GetLayerByLuminance(noiseValues[x, y] * 1.0f);
                    baseColor = layer.Color;
                    lum = 200f;
                }

                AddColorToData(data, baseColor, useGrid ? GridLuminance : lum, useGrid);
            }
        }

        if (LogMinMax)
        {
            List<float> max = lvals.OrderByDescending(f => f).Take(3).ToList();
            List<float> min = lvals.OrderBy(f => f).Take(3).ToList();

            Log.Info("-------");
            Log.Info("=======");
            min.ForEach(f => Log.Info($"Min: {f}"));
            Log.Info("=======");
            Log.Info("-------");
            Log.Info("=======");
            max.ForEach(f => Log.Info($"Max: {f}"));
            Log.Info("=======");
            Log.Info("-------");
        }

        return data;
    }

}