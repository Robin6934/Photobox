using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.ImageViewer;
public interface IImageViewer
{
    public Task ShowImage(Image<Rgb24> image);
}
