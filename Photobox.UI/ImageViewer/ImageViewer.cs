using Photobox.LocalServer.RestApi.Api;
using Photobox.UI.Windows;

namespace Photobox.UI.ImageViewer;
internal class ImageViewer(IPhotoboxApi photobox, ISettingsApi settings) : IImageViewer
{
    private readonly IPhotoboxApi photoboxApi = photobox;

    private readonly ISettingsApi settingsApi = settings;

    private bool printingEnabled = default;

    public async Task ShowImage(string imagePath)
    {
        printingEnabled = settingsApi.ApiSettingsPrintingEnabledGet().PrintingEnabled;

        var window = new ImageViewWindow(imagePath, photoboxApi, printingEnabled);

        var tcs = new TaskCompletionSource<ImageViewResult>();

        window.Closed += (o, r) => tcs.SetResult(r);

        window.Show();

        var result = await tcs.Task;

        Task task = result switch
        {
            ImageViewResult.Save => photoboxApi.ApiPhotoboxSaveImagePathGetAsync(imagePath),
            ImageViewResult.Print => photoboxApi.ApiPhotoboxPrintImagePathGetAsync(imagePath),
            ImageViewResult.Delete => photoboxApi.ApiPhotoboxDeleteImagePathGetAsync(imagePath),
            _ => throw new NotImplementedException(),
        };

        await task;
    }
}
