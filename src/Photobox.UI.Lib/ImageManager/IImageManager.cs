using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.ImageManager;

public interface IImageManager
{
    public Task PrintAndSaveAsync(Image<Rgb24> image);

    public Task SaveAsync(Image<Rgb24> image);

    public Task DeleteAsync(Image<Rgb24> image);
}
