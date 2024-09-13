using Microsoft.Extensions.Options;
using Photobox.LocalServer.RestApi.Model;
using System.Drawing;
using System.Drawing.Printing;

namespace Photobox.Lib.Printer;
public class Printer(IOptionsMonitor<PhotoboxConfig> options) : IPrinter
{
    readonly PhotoboxConfig config = options.CurrentValue;
    public async Task PrintAsync(string imagePath)
    {
        Bitmap image = new(imagePath);

        PrinterSettings printerSettings = new()
        {
            PrinterName = config.PrinterName
        };

        using PrintDocument pd = new()
        {
            PrinterSettings = printerSettings
        };

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
