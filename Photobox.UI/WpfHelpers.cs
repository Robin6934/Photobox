using Microsoft.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Photobox.WpfHelpers;

public static partial class BitmapToBitmapSource
{
    public static BitmapSource ToBitmapSource(this Bitmap source)
    {
        var bitmapData = source.LockBits(
        new System.Drawing.Rectangle(0, 0, source.Width, source.Height),
        System.Drawing.Imaging.ImageLockMode.ReadOnly, source.PixelFormat);

        var bitmapSource = BitmapSource.Create(
           bitmapData.Width, bitmapData.Height, 96.0, 96.0, PixelFormats.Bgr24, null,
           bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

        source.UnlockBits(bitmapData);

        return bitmapSource;
    }

    public static BitmapSource ToBitmapSource<TPixel>(this Image<TPixel> image) where TPixel: unmanaged, IPixel<TPixel>
    {
        using var stream = new MemoryStream();

        // Save the Image<Rgb24> to a stream in a format BitmapSource understands (e.g., PNG)
        image.Save(stream, new JpegEncoder());
        stream.Seek(0, SeekOrigin.Begin);

        // Create a BitmapImage and load the MemoryStream data
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();

        // Freeze the BitmapImage to make it cross-thread accessible
        bitmapImage.Freeze();

        return bitmapImage;
    }

    public static ImageBrush ToBitmapSource(this Stream stream)
    {
        // Ensure the stream is at the beginning before reading it.
        stream.Seek(0, SeekOrigin.Begin);

        BitmapImage bitmap = new();

        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load the data immediately, so stream doesn't stay open.
        bitmap.StreamSource = stream;
        bitmap.EndInit();

        bitmap.Freeze(); // Freeze the bitmap to make it thread-safe and immutable

        return new ImageBrush(bitmap);
    }
}