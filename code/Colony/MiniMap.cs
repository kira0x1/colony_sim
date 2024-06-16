namespace Kira;

using System.Globalization;

[Category("Kira")]
public partial class MiniMap : Component, INoiseRenderer
{
    [Property] public Color grassColor { get; set; }
    [Property] public List<LayerInfo> Layers { get; set; } = new();
    [Property, Range(1, 512)] public int SpriteSize { get; set; } = 512;

    // Luminance
    [Group("Luminance"), Property, Range(0, 15)] public float Brightness { get; set; } = 255;
    [Group("Luminance"), Property, Range(0, 255)] public int MinLuminance { get; set; } = 100;
    [Group("Luminance"), Property, Range(0, 255)] public int MaxLuminance { get; set; } = 255;

    protected override void OnValidate()
    {
        if (SpriteSize > 512) SpriteSize = 512;
        var noise = CreateNoise(SpriteSize);
        CreateTexture(noise);
    }

    private static void AddColorToData(List<byte> data, Color c, float lum)
    {
        Color finalColor = c.ToHsv().WithValue(lum).ToColor();

        float r = finalColor.r.CeilToInt().Clamp(0, 255);
        float g = finalColor.g.CeilToInt().Clamp(0, 255);
        float b = finalColor.b.CeilToInt().Clamp(0, 255);

        byte rByte = byte.Parse(r.ToString(CultureInfo.InvariantCulture));
        byte gByte = byte.Parse(g.ToString(CultureInfo.InvariantCulture));
        byte bByte = byte.Parse(b.ToString(CultureInfo.InvariantCulture));

        data.Add(rByte);
        data.Add(gByte);
        data.Add(bByte);
        data.Add(255);
    }

    public void CreateTexture(float[,] noiseValues)
    {
        if (SpriteSize > 512) SpriteSize = 512;

        // Create a compute shader from a .shader file
        var computeShader = new ComputeShader("code/shaders/map_shader");
        var sp = GameObject.Components.GetOrCreate<SpriteRenderer>();

        sp.Size = SpriteSize;

        List<byte> data = GetColorInfo(noiseValues);

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

        if (!sp.IsValid())
        {
            Log.Info("Could not find sprite renderer");
            return;
        }

        sp.Texture = texture;
    }

    private bool LogMinMax { get; set; }

    private List<byte> GetColorInfo(float[,] noiseValues)
    {
        List<byte> data = new List<byte>();

        List<float> lvals = new List<float>();

        for (int x = 0; x < SpriteSize; x++)
        {
            for (int y = 0; y < SpriteSize; y++)
            {
                float lum = noiseValues[x, y] * Brightness;
                lvals.Add(lum);
                lum = lum.CeilToInt().Clamp(MinLuminance, MaxLuminance);

                AddColorToData(data, grassColor, lum);
            }
        }

        if (LogMinMax)
        {
            List<float> max = lvals.OrderByDescending(x => x).Take(3).ToList();
            List<float> min = lvals.OrderBy(x => x).Take(3).ToList();

            Log.Info("-------");
            Log.Info("=======");
            min.ForEach(x => Log.Info($"Min: {x}"));
            Log.Info("=======");
            Log.Info("-------");
            Log.Info("=======");
            max.ForEach(x => Log.Info($"Max: {x}"));
            Log.Info("=======");
            Log.Info("-------");
        }

        return data;
    }
}