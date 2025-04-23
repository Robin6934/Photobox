using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.ImageUploadService;

public interface IImageUploadService
{
    public Task UploadImageAsync(string name, Image<Rgb24> image);
}
