using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Web.StorageProvider;

public interface IStorageProvider
{
    public Task StoreImageAsync(Image<Rgb24> image, string name);

    public Task<Image<Rgb24>> GetImageAsync(string name);

    public Task DeleteImageAsync(string name);
}
