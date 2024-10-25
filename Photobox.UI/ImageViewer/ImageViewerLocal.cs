using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.Lib.ConfigModels;
using Photobox.Lib.PhotoManager;
using Photobox.Lib.Printer;
using Photobox.UI.Windows;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Photobox.UI.ImageViewer;
public class ImageViewerLocal(ILogger<ImageViewerLocal> logger, IOptionsMonitor<PhotoboxConfig> monitor, IImageManager imageManager, IPrinter printer) : IImageViewer
{
    private readonly ILogger<ImageViewerLocal> logger = logger;

    private readonly PhotoboxConfig photoboxConfig = monitor.CurrentValue;

    private readonly IImageManager imageManager = imageManager;

    private readonly bool printerEnabled = printer.Enabled;

    public async Task ShowImage(Image<Rgb24> image)
    {
        var window = new ImageViewWindow(image, printerEnabled);

        var tcs = new TaskCompletionSource<ImageViewResult>();

        window.Closed += (o, r) => tcs.SetResult(r);

        window.Show();

        var result = await tcs.Task;

        switch (result)
        {
            case ImageViewResult.Save:
                imageManager.Save(image);
                break;

            case ImageViewResult.Print:
                await imageManager.PrintAndSaveAsync(image);
                break;

            case ImageViewResult.Delete:
                imageManager.Delete(image);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
