using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.Lib.ConfigModels;
using Photobox.Lib.ImageHandler;
using Photobox.Lib.PhotoManager;
using Photobox.Lib.Printer;
using Photobox.UI.Windows;
using Photobox.Web.RestApi.Api;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

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
