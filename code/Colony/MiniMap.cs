namespace Kira;

using System.Globalization;

public class LayerInfo
{
    [Property]
    public Color Color { get; set; } = Color.White;

    [Property, Range(0, 1)]
    public float Start { get; set; } = 0;

    [Property, Range(0, 1)]
    public float End { get; set; } = 1;

    [Property, Range(0, 255)]
    public int MinLuminance { get; set; } = 100;

    [Property, Range(0, 255)]
    public int MaxLuminance { get; set; } = 255;
}

[Category("Kira")]
public class MiniMap : Component, INoiseRenderer
{
    [Property]
    public Color grassColor { get; set; }

    private bool hasTexture { get; set; }
    private Texture tx { get; set; }

    public const int Resolution = 1;

    [Property]
    public List<LayerInfo> Layers { get; set; } = new();


    [Property, Range(1, 512)]
    public int SpriteSize { get; set; } = 512;

    [Property, Range(0, 255)]
    public float BaseMultiplier { get; set; } = 255;

    [Property, Range(0, 255)]
    public int MinLuminance { get; set; } = 100;

    [Property, Range(0, 255)]
    public int MaxLuminance { get; set; } = 255;


    protected override void OnValidate()
    {
        var generator = Components.Get<NoiseGenerator>();

        if (SpriteSize > 512) SpriteSize = 512;

        if (generator.IsValid())
        {
            generator.UpdateNoise();
        }
    }

    public void CreateTexture(float[,] noiseValues)
    {
        if (SpriteSize > 512) SpriteSize = 512;

        hasTexture = false;

        // Create a compute shader from a .shader file
        var computeShader = new ComputeShader("code/shaders/map_shader");
        var sp = GameObject.Components.GetOrCreate<SpriteRenderer>();

        sp.Size = SpriteSize;

        List<byte> data = new List<byte>();

        for (int x = 0; x < SpriteSize; x++)
        {
            for (int y = 0; y < SpriteSize; y++)
            {
                float lum = noiseValues[x, y] * BaseMultiplier;
                lum = lum.CeilToInt().Clamp(MinLuminance, MaxLuminance);


                var darkened = grassColor.ToHsv().WithValue(lum).ToColor();
                float r = (darkened.r).CeilToInt().Clamp(0, 255);
                float g = (darkened.g).CeilToInt().Clamp(0, 255);
                float b = (darkened.b).CeilToInt().Clamp(0, 255);

                byte rByte = byte.Parse(r.ToString(CultureInfo.InvariantCulture));
                byte gByte = byte.Parse(g.ToString(CultureInfo.InvariantCulture));
                byte bByte = byte.Parse(b.ToString(CultureInfo.InvariantCulture));


                // byte val = byte.Parse(lum.ToString(CultureInfo.InvariantCulture));

                data.Add(rByte);
                data.Add(gByte);
                data.Add(bByte);
                data.Add(255);
            }
        }


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

        tx = texture;
        hasTexture = true;

        if (!sp.IsValid())
        {
            Log.Info("Could not find sprite renderer");
            return;
        }

        sp.Texture = tx;
    }
}