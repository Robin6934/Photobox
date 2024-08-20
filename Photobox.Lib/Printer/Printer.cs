using System.Drawing;
using System.Drawing.Printing;

namespace Photobox.Lib.Printer;
public class Printer : IPrinter
{
    public async Task PrintAsync(string imagePath)
    {
        Bitmap image = new(imagePath);
        using PrintDocument pd = new();
        pd.PrintPage += (sender, e) =>
        {
            float width = e.PageSettings.PrintableArea.Width;
            float height = e.PageSettings.PrintableArea.Height;
            e.Graphics?.DrawImage(image, 0.0f, 0.0f, height, width);
        };
        pd.Print();
        await Task.CompletedTask;
    }
}
