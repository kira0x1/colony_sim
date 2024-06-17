namespace Kira;

[GameResource("Noise Layers Config", "nlayer", "Stores the layers to display color by luminance", Icon = "terrain")]
public class LayerConfig : GameResource
{
    [Property]
    public List<LayerInfo> Layers { get; set; } = new List<LayerInfo>();
}