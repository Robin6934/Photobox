using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.ImageHandler;

public interface IImageHandler
{
    public Image<Rgb24> DrawOnImage(Image<Rgb24> image);
}
