using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.UI.Lib.ConfigModels;
using Photobox.UI.Lib.ImageUploadService;
using Photobox.UI.Lib.Printer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.ImageManager;
public class ImageManager(ILogger<ImageManager> logger, IOptionsMonitor<PhotoboxConfig> options, IPrinter printer, IImageUploadService imageUploadService) : IImageManager
{
    private readonly ILogger<ImageManager> logger = logger;

    private readonly IPrinter printer = printer;

    private readonly IOptionsMonitor<PhotoboxConfig> photoboxConfigMonitor = options;

    private readonly IImageUploadService imageUploadService = imageUploadService;

    public async Task DeleteAsync(Image<Rgb24> image)
    {
        if (photoboxConfigMonitor.CurrentValue.StoreDeletedImages)
        {
            string imageName = Folders.NewImageName;

            string newImagePath = Path.Combine(
                Folders.PhotoboxBaseDir,
                Folders.Deleted,
                imageName);

            await image.SaveAsJpegAsync(newImagePath);

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

        await image.SaveAsJpegAsync(newImagePath);

        await imageUploadService.UploadImageAsync(imageName, image);

        logger.LogInformation("Stored Saved image under path {imagePath}", imageName);
    }
}
