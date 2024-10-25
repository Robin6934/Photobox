using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Photobox.Lib.Printer;
public interface IPrinter
{
    public bool Enabled { get; }

    public List<string> ListPrinters();

    public bool SetPrinter(string printerName);

    public void SetPrinterEnabled(PrinterEnabledOptions printerEnabled);

    public Task PrintAsync(Image<Rgb24> imagePath);
}
