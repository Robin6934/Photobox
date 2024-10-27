using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.Lib.ConfigModels;
using Photobox.Lib.Printer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;

namespace Photobox.Lib.PhotoManager;
public class ImageManager(ILogger<ImageManager> logger, IOptionsMonitor<PhotoboxConfig> options, IPrinter printer) : IImageManager
{
    private readonly ILogger<ImageManager> logger = logger;

    private readonly IPrinter printer = printer;

    private readonly IOptionsMonitor<PhotoboxConfig> photoboxConfigMonitor = options;

    public async Task DeleteAsync(Image<Rgb24> image)
    {
        if (photoboxConfigMonitor.CurrentValue.StoreDeletedImages)
        {
            string imageName = Folders.NewImageName;

            string newImagePath = Path.Combine(
                Folders.PhotoboxBaseDir,
                Folders.Deleted,
                imageName);

            await image.SaveAsync(newImagePath, new JpegEncoder());

            logger.LogInformation("Stored Deleted image under path {imagePath}", imageName);
        }
    }

    public async Task PrintAndSaveAsync(Image<Rgb24> image)
    {
        await SaveAsync(image);

        await printer.PrintAsync(image);
    }

    public async Task SaveAsync(Image<Rgb24> image)
    {
        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        await image.SaveAsync(newImagePath, new JpegEncoder());

        logger.LogInformation("Stored Saved image under path {imagePath}", imageName);
    }
}
