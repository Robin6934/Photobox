namespace Photobox.Web.Client.Pages.Components;

public class ImageObject
{
    public string Content { get; set; } = ""; // Image URL or Text
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    
    public double Scale { get; set; } = 1.0d;
}
