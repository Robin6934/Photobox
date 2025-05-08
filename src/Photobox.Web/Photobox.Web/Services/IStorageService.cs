using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Web.Services;

public interface IStorageService
{
    public Task StoreImageAsync(
        Image<Rgb24> image,
        string name,
        CancellationToken cancellationToken = default
    );

    public Task<Image<Rgb24>> GetImageAsync(
        string name,
        CancellationToken cancellationToken = default
    );

    public Task DeleteImageAsync(string name, CancellationToken cancellationToken = default);

    public Task DeleteImagesAsync(
        IEnumerable<string> images,
        CancellationToken cancellationToken = default
    );

    public Task<string> GetPreSignedUrl(string name, TimeSpan validFor);
}
