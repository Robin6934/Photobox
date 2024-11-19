using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib.Extensions;
using Photobox.Web.RestApi.Api;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.ImageSyncService;
public class ImageSyncService(ILogger<ImageSyncService> logger, IImageApi imageApi) : BackgroundService
{
    private readonly ILogger<ImageSyncService> logger = logger;

    private readonly IImageApi imageApi = imageApi;

    public async Task SyncImagesAsync()
    {
        List<string> uploadedImages = await imageApi.ApiImageListImagesGetAsync();

        List<string> localImages = [.. Directory.GetFiles(Folders.GetPath(Folders.Photos))];

        //both lists contain the same data
        if (uploadedImages.Count == localImages.Count
            && !uploadedImages.Except(localImages).Any()
            && !localImages.Except(uploadedImages).Any())
        {
            return;
        }

        foreach (var imageName in localImages)
        {
            if (!uploadedImages.Contains(imageName))
            {
                logger.LogInformation(
                    "The image with the name {imageName} is not on the server and will get uploaded."
                    , imageName);

                Image<Rgb24> image = Image.Load<Rgb24>(Path.Combine(Folders.Photos, imageName));

                Stream imageStream = await image.ToJpegStreamAsync();

                await imageApi.ApiImageUploadImagePostAsync(imageName, imageStream);
            }
        }

        foreach (var imageName in uploadedImages)
        {
            if (!localImages.Contains(imageName))
            {
                logger.LogInformation(
                    "The image with the name {imageName} does not exist locally and will get downloaded."
                    , imageName);

                var result = await imageApi.ApiImageGetImageImageNameGetAsync(imageName);

                var image = Image.Load<Rgb24>(result);

                await image.SaveAsJpegAsync(Folders.GetPath(Folders.Photos, imageName));
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SyncImagesAsync();

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
