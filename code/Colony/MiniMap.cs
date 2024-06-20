using System.Globalization;

namespace Kira;

[Category("Kira")]
public sealed partial class MiniMap : Component
{
    [Property, Group("Base")] public bool DrawOnSprite { get; set; }
    [Property, Group("Base")] public bool DrawGridOnSprite { get; set; }
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

    private List<LayerInfo> LayersChosen = new List<LayerInfo>();


    protected override void OnValidate()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (SpriteSize > 512) SpriteSize = 512;
        LayersChosen = UseLayerConfig ? LayerConfig.Layers : Layers;

        if (!GameObject.IsValid())
        {
            // Log.Warning("this gameobject not valid!?");
            return;
        }

        SpriteRenderer sp = GameObject.Components.Get<SpriteRenderer>(true);

        if (!sp.IsValid())
        {
            Log.Info("Could not find sprite renderer");
            return;
        }

        if (!DrawOnSprite && !DrawGridOnSprite)
        {
            sp.Enabled = false;
            return;
        }

        sp.Enabled = true;

        curMsgLogged = 0;
        sp.Size = SpriteSize;

        if (DrawOnSprite && !DrawGridOnSprite)
        {
            var tx = CreateMiniMapTexture();
            sp.Texture = tx;
        }
        else if (!DrawOnSprite && DrawGridOnSprite)
        {
            var tx = CreateGridTexture();
            sp.Texture = tx;
        }
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

    public Texture CreateMiniMapTexture()
    {
        float[,] noiseData = CreateNoise();
        List<byte> mapData = GetColorInfo(noiseData);
        return CreateTexture(mapData);
    }

    public Texture CreateFinalTexture()
    {
        float[,] noiseData = CreateNoise();
        List<byte> mapData = GetColorInfo(noiseData);
        OverlayGridData(mapData);
        return CreateTexture(mapData);
    }

    public Texture CreateGridTexture()
    {
        List<byte> gridData = CreateGridData();
        return CreateTexture(gridData);
    }

    private Texture CreateTexture(List<byte> data)
    {
        // Create a compute shader from a .shader file
        var computeShader = new ComputeShader("code/shaders/map_shader");

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
            if (curMsgLogged < 2)
            {
                Log.Info($"layer for {lum} not found");
                curMsgLogged++;
            }
        }

        return layer;
    }

    int curMsgLogged { get; set; } = 0;

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

                Color baseColor = grassColor;


                if (UseLayers)
                {
                    var layer = GetLayerByLuminance(noiseValues[x, y] * 1.0f);
                    baseColor = layer.Color;
                }

                AddColorToData(data, baseColor, lum, false);
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