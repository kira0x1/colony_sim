namespace Kira;

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