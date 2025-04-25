using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.UI.Lib.ConfigModels;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Photobox.UI.Lib.ImageHandler;

public class ImageHandler(
    ILogger<ImageHandler> logger,
    IOptionsMonitor<PhotoboxConfig> optionsMonitor
) : IImageHandler
{
    private readonly ILogger<ImageHandler> logger = logger;

    private readonly IOptionsMonitor<PhotoboxConfig> optionsMonitor = optionsMonitor;

    public Image<Rgb24> DrawOnImage(Image<Rgb24> image)
    {
        string text = optionsMonitor.CurrentValue.TextOnPicture;
        const float TextPadding = 18f;
        const string TextFont = "Arial";
        const float TextFontSize = 64f;

        if (!SystemFonts.TryGet(TextFont, out FontFamily fontFamily))
            throw new Exception($"Couldn't find font {TextFont}");

        var font = fontFamily.CreateFont(TextFontSize, FontStyle.Regular);

        var options = new TextOptions(font) { Dpi = 72, KerningMode = KerningMode.Standard };

        var rect = TextMeasurer.MeasureSize(text, options);

        // Create an image with RGBA32 for transparency support
        using Image<Rgba32> imageWithText = new(image.Width, image.Height);
        imageWithText.Mutate(x => x.BackgroundColor(Color.Transparent));

        // Draw the text with RGBA color (including transparency)
        imageWithText.Mutate(x =>
            x.DrawText(
                text,
                font,
                Color.White.WithAlpha(0.93f), // Semi-transparent white
                new PointF(
                    imageWithText.Width - rect.Width - TextPadding,
                    imageWithText.Height - rect.Height - TextPadding
                )
            )
        );

        // Save the image as PNG to preserve transparency
        image.Mutate(x => x.DrawImage(imageWithText, 1.0f));

        return image;
    }
}
