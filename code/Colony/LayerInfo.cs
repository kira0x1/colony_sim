namespace Kira;

public class LayerInfo
{
    [Property]
    public Color Color { get; set; } = Color.White;

    [Property, Range(0, 1000)]
    public float Start { get; set; } = 0;

    [Property, Range(0, 1000)]
    public float End { get; set; } = 100;
}