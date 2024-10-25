using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Lib.PhotoManager;
public interface IImageManager
{
    public Task PrintAndSaveAsync(Image<Rgb24> image);

    public void Save(Image<Rgb24> image);

    public void Delete(Image<Rgb24> image);
}
