using SixLabors.ImageSharp;

namespace Photobox.Lib.Helper;

public static class ImageHelper
{
    public static AspectRatio GetAspectRatio(this Image source)
    {
        return new AspectRatio(source.Width, source.Height);
    }
}
