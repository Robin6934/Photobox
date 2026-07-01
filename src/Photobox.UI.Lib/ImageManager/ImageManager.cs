using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Photobox.UI.Lib.ConfigModels;
using Photobox.UI.Lib.ImageUploadService;
using Photobox.UI.Lib.Printer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.ImageManager;

public class ImageManager(
    ILogger<ImageManager> logger,
    IOptionsMonitor<PhotoboxConfig> options,
    IPrinter printer,
    IImageUploadService imageUploadService,
    IFileSystem fileSystem
) : IImageManager
{
    private readonly ILogger<ImageManager> logger = logger;

    private readonly IPrinter printer = printer;

    private readonly IOptionsMonitor<PhotoboxConfig> photoboxConfigMonitor = options;

    private readonly IImageUploadService imageUploadService = imageUploadService;

    private readonly IFileSystem fileSystem = fileSystem;

    public async Task DeleteAsync(Image<Rgb24> image)
    {
        if (photoboxConfigMonitor.CurrentValue.StoreDeletedImages)
        {
            string imageName = Folders.NewImageName;

            string newImagePath = Path.Combine(Folders.PhotoboxBaseDir, Folders.Deleted, imageName);

            await using Stream fileStream = fileSystem.FileStream.New(
                newImagePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None
            );

            await image.SaveAsJpegAsync(fileStream);

            logger.LogInformation("Stored Deleted image under path {imagePath}", imageName);
        }
    }

    public Task PrintAndSaveAsync(Image<Rgb24> image)
    {
        var saveTask = SaveAsync(image);

        var printTask = printer.PrintAsync(image);

        return Task.WhenAll(saveTask, printTask);
    }

    public async Task SaveAsync(Image<Rgb24> image)
    {
        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(Folders.PhotoboxBaseDir, Folders.Photos, imageName);

        await using Stream fileStream = fileSystem.FileStream.New(
            newImagePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None
        );

        await image.SaveAsJpegAsync(fileStream);

        await imageUploadService.UploadImageAsync(imageName, image);

        logger.LogInformation("Stored Saved image under path {imagePath}", imageName);
    }
}
