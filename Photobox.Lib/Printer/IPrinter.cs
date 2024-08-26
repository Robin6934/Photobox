namespace Photobox.Lib.Printer;
public interface IPrinter
{
    public Task PrintAsync(string imagePath, string printerName);
}
