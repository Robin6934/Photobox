using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib.Extensions;
using Photobox.Web.RestApi.Api;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;

namespace Photobox.UI.Lib.ImageUploadService;

public class ImageUploadService(ILogger<ImageUploadService> logger, IImageApi imageApi) : IImageUploadService
{
    private readonly ILogger<ImageUploadService> logger = logger;

    private readonly IImageApi imageApi = imageApi;

    private ConcurrentDictionary<string, Image<Rgb24>> images = [];

    public async Task UploadImageAsync(string name, Image<Rgb24> image)
    {
        images.TryAdd(name, image);

        await UploadImages();
    }

    private async Task UploadImages()
    {
        foreach((string name, Image<Rgb24> image) in images)
        {
            using var imageStream = await image.ToJpegStreamAsync();

            var result = await imageApi.ApiImageUploadImagePostWithHttpInfoAsync(name, imageStream);

            if (result.ErrorText is null)
            {
                images.Remove(name, out _);
            }
        }
    }
}
