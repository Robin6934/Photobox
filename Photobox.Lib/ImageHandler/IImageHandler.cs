using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Lib.ImageHandler;

public interface IImageHandler
{
    public void DrawOnImage(Image<Rgb24> image);
}
