namespace Photobox.Web.Client.Pages.Components;

public class ImageObject
{
    public string Type { get; set; } = ""; // "Text" or "Image"
    public string Content { get; set; } = ""; // Image URL or Text
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int Width { get; set; } = 50;
    public int Height { get; set; } = 50;
    public int FontSize { get; set; } = 16;
}
