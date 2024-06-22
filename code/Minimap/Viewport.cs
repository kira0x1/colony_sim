namespace Kira;

public class Viewport
{
    public Rect ViewRect { get; set; }
    public Vector2 ViewSize = new Vector2(256, 256);
    public Vector2 Position { get; set; } = Vector2.Zero;

    public float Width => ViewRect.Width;
    public float Height => ViewRect.Height;

    public Viewport()
    {
        ViewRect = new Rect(Position, ViewSize);
    }

    public Viewport(float x = 0, float y = 0, float width = 256, float height = 256)
    {
        ViewRect = new Rect(x, y, width, height);
    }
}