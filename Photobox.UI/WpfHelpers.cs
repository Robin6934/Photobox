using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Photobox.WpfHelpers;

public static partial class BitmapToBitmapSource
{
    public static BitmapSource ToBitmapSource(this Bitmap source)
    {
        var bitmapData = source.LockBits(
        new Rectangle(0, 0, source.Width, source.Height),
        System.Drawing.Imaging.ImageLockMode.ReadOnly, source.PixelFormat);

        var bitmapSource = BitmapSource.Create(
           bitmapData.Width, bitmapData.Height, 96, 96, PixelFormats.Bgr24, null,
           bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

        source.UnlockBits(bitmapData);

        return bitmapSource;
    }
}