using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.Printer;
public interface IPrinter
{
    public bool Enabled { get; }

    public List<string> ListPrinters();

    public bool SetPrinter(string printerName);

    public void SetPrinterEnabled(PrinterEnabledOptions printerEnabled);

    public Task PrintAsync(Image<Rgb24> imagePath);
}
