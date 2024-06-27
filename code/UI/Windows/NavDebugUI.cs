namespace Kira.UI;

using Sandbox.UI;
using Sandbox.UI.Construct;
using Util;

public class NavDebugUI : BaseNavWindow
{
    public Panel MapContainer { get; protected set; }

    public NavDebugUI()
    {
        StyleSheet.Load("UI/Windows/NavDebugUI.cs.scss");
        MapContainer = Add.Panel("map");

        var gridTexture = new GridTexture();
        var img = new Image();
        img.Texture = gridTexture.CreateGridTexture();
        img.AddClass("grid");
        MapContainer.AddChild(img);
    }
}