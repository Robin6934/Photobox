using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.Lib.ConfigModels;
using Photobox.Lib.ImageHandler;
using Photobox.Lib.PhotoManager;
using Photobox.Lib.Printer;
using Photobox.UI.Windows;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.ImageViewer;
public class ImageViewerLocal(ILogger<ImageViewerLocal> logger, IOptionsMonitor<PhotoboxConfig> monitor, IImageManager imageManager, IPrinter printer, IImageHandler imageHandler) : IImageViewer
{
    private readonly ILogger<ImageViewerLocal> logger = logger;

    private readonly IOptionsMonitor<PhotoboxConfig> photoboxConfig = monitor;

    private readonly IImageManager imageManager = imageManager;

    private readonly bool printerEnabled = printer.Enabled;

    private readonly IImageHandler imageHandler = imageHandler;

    public async Task ShowImage(Image<Rgb24> image)
    {
        var window = new ImageViewWindow(image, printerEnabled);

        var tcs = new TaskCompletionSource<ImageViewResult>();

        window.Closed += (o, r) => tcs.SetResult(r);

        window.Show();

        var result = await tcs.Task;

        var imageWithText = imageHandler.DrawOnImage(image);

        switch (result)
        {
            case ImageViewResult.Save:
                await imageManager.SaveAsync(imageWithText);
                break;

            case ImageViewResult.Print:
                await imageManager.PrintAndSaveAsync(imageWithText);
                break;

            case ImageViewResult.Delete:
                await imageManager.DeleteAsync(imageWithText);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
