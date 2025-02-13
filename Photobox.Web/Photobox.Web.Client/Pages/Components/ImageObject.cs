namespace Photobox.Web.Client.Pages.Components;

public class ImageObject
{
    public string Content { get; set; } = ""; // Image URL or Text
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    
    public double Scale { get; set; } = 1.0d;
    
    public int ZIndex { get; set; } = 0;
}
