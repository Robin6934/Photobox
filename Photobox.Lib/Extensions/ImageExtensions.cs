using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Lib.Extensions;

public static class ImageExtensions
{
    public static async Task<Stream> ToJpegStreamAsync<TPixel>(this Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        MemoryStream memoryStream = new();

        await image.SaveAsJpegAsync(memoryStream);

        memoryStream.Position = 0;

        return memoryStream;
    }
}
