using Photobox.LocalServer.RestApi.Api;

namespace Photobox.UI;
internal class ImageViewer : IImageViewer
{
    private readonly IPhotoboxApi photoboxApi = default!;

    private readonly ISettingsApi settingsApi = default!;

    private readonly bool printingEnabled = default;

    public ImageViewer(IPhotoboxApi photobox, ISettingsApi settings)
    {
        photoboxApi = photobox;
        settingsApi = settings;

        printingEnabled = settings.ApiSettingsPrintingEnabledGet().PrintingEnabled;
    }

    public async Task ShowImage(string imagePath)
    {
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
