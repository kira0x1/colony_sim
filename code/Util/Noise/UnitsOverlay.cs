namespace Kira;

[Category("Kira")]
public class UnitsOverlay : Component
{
    [Group("Generator"), Property] public int SpriteSize { get; set; } = 128;
    [Group("Generator"), Property, Range(0f, 3f)] public float Scale { get; set; } = 1f;
    [Group("Generator"), Property, Range(1f, 5f)] public float ZoomIn { get; set; } = 1f;
    [Group("Generator"), Property, Range(1f, 10f)] public float ZoomOut { get; set; } = 1f;
    [Group("Generator"), Property] public Color BackgroundColor { get; set; } = Color.Gray;

    public int[,] CreateUnitGrid(int pixelSize = 512)
    {
        float finalZoom = ZoomIn / ZoomOut;
        float finalScale = Scale / finalZoom;

        int[,] data = new int[pixelSize, pixelSize];

        for (int y = 0; y < pixelSize; y++)
        {
            for (int x = 0; x < pixelSize; x++)
            {
                float px = x * finalScale;
                float py = y * finalScale;
                data[y, x] = 0;
            }
        }

        return data;
    }

    private Texture CreateTexture(List<byte> data)
    {
        // Create a compute shader from a .shader file
        var computeShader = new ComputeShader("code/shaders/map_shader");

        // Create a texture for the compute shader to use
        var texture = Texture.Create(SpriteSize, SpriteSize)
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