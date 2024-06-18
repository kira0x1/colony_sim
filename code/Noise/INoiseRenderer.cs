namespace Kira;

public interface INoiseRenderer
{
    public bool Enabled { get; set; }
    public int SpriteSize { get; set; }
    public Texture CreateTexture();
}