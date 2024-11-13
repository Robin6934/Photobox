using Microsoft.Extensions.Logging;
using Photobox.UI.Lib.Printer;
using Photobox.UI.Windows;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.ImageViewer;
public class ImageViewerLocal(
    ILogger<ImageViewerLocal> logger,
    IPrinter printer) : IImageViewer
{
    private readonly ILogger<ImageViewerLocal> logger = logger;

    private readonly bool printerEnabled = printer.Enabled;

    public async Task<ImageViewResult> ShowImage(Image<Rgb24> image)
    {
        var window = new ImageViewWindow(image, printerEnabled);

        var tcs = new TaskCompletionSource<ImageViewResult>();

        window.Closed += (o, r) => tcs.SetResult(r);

        window.Show();

        var result = await tcs.Task;

        return result;
    }
}
