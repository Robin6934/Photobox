using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Photobox.Lib.Extensions;
using Photobox.Lib.RestApi;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.ImageUploadService;

public class ImageUploadService(ILogger<ImageUploadService> logger, IImageClient client)
    : IImageUploadService
{
    private readonly ILogger<ImageUploadService> _logger = logger;

    private readonly IImageClient _client = client;

    private ConcurrentDictionary<string, Image<Rgb24>> _images = [];

    public async Task UploadImageAsync(string name, Image<Rgb24> image)
    {
        _images.TryAdd(name, image);

        await UploadImages();
    }

    private async Task UploadImages()
    {
        foreach ((string name, Image<Rgb24> image) in _images)
        {
            await using var imageStream = await image.ToJpegStreamAsync();

            string mimeType = image.Metadata.DecodedImageFormat?.DefaultMimeType ?? "image/jpeg";

            var fileParameter = new FileParameter(imageStream, name, mimeType);

            try
            {
                var result = await _client.UploadImageAsync(fileParameter);

                _logger.LogDebug("Uploaded image with name {imageName}.", result.FileName);

                _images.Remove(name, out _);
            }
            catch (System.Exception)
            {
                // Do nothing, as the image will retry the upload next round only stop the loop
                break;
            }
        }
    }
}
