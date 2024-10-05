using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.Lib.ConfigModels;
using Photobox.Lib.PhotoManager;
using Photobox.Lib.Printer;
using Photobox.UI.Windows;

namespace Photobox.UI.ImageViewer;
public class ImageViewerLocal(ILogger<ImageViewerLocal> logger, IOptionsMonitor<PhotoboxConfig> monitor, IImageManager imageManager, IPrinter printer) : IImageViewer
{
    private readonly ILogger<ImageViewerLocal> logger = logger;

    private readonly PhotoboxConfig photoboxConfig = monitor.CurrentValue;

    private readonly IImageManager imageManager = imageManager;

    private readonly bool printerEnabled = printer.Enabled;

    public async Task ShowImage(string imagePath)
    {
        var window = new ImageViewWindow(imagePath, printerEnabled);

        var tcs = new TaskCompletionSource<ImageViewResult>();

        window.Closed += (o, r) => tcs.SetResult(r);

        window.Show();

        var result = await tcs.Task;

        switch (result)
        {
            case ImageViewResult.Save:
                imageManager.Save(imagePath);
                break;

            case ImageViewResult.Print:
                await imageManager.PrintAndSaveAsync(imagePath);
                break;

            case ImageViewResult.Delete:
                imageManager.Delete(imagePath);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
