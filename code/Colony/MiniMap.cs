namespace Kira;

using System.Globalization;

public class GridSettings
{
    [Property]
    public float gridGap = 4;


}

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

    [Group("Grid"), Property, Range(0, 60)] public float GridGap { get; set; } = 4;

    [Group("Grid"), Property] public Color GridColor { get; set; } = Color.White;
    [Group("Grid"), Property, Range(0, 255)] public float GridLuminance { get; set; } = 100f;

    protected override void OnValidate()
    {
        if (SpriteSize > 512) SpriteSize = 512;
        CreateNoise(SpriteSize);
        CreateTexture();
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
            finalColor = hsvColor.WithValue(lum).ToColor();
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

    public void CreateTexture()
    {
        if (SpriteSize > 512) SpriteSize = 512;

        // Create a compute shader from a .shader file
        var computeShader = new ComputeShader("code/shaders/map_shader");
        var sp = GameObject.Components.GetOrCreate<SpriteRenderer>();

        sp.Size = SpriteSize;

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
                lum = lum.CeilToInt().Clamp(MinLuminance, MaxLuminance);

                var baseColor = grassColor;

                if (x % GridGap < 1 || y % GridGap < 1)
                {
                    baseColor = GridColor;
                    AddColorToData(data, baseColor, GridLuminance, true);
                }
                else
                {
                    AddColorToData(data, baseColor, lum);
                }

                if (LogMinMax)
                {
                    lvals.Add(lum);
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
        }


        return data;
    }
}