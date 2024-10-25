using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.Lib.ConfigModels;
using Photobox.Lib.Printer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System.Data;
using System.Diagnostics;

namespace Photobox.Lib.PhotoManager;
public class ImageManager(ILogger<ImageManager> logger, IOptionsMonitor<PhotoboxConfig> options, IPrinter printer) : IImageManager
{
    private readonly ILogger<ImageManager> logger = logger;

    private readonly IPrinter printer = printer;

    private readonly IOptionsMonitor<PhotoboxConfig> photoboxConfigMonitor = options;

    public void Delete(Image<Rgb24> image)
    {
        if(photoboxConfigMonitor.CurrentValue.StoreDeletedImages)
        {
            string imageName = Folders.NewImageName;

            string newImagePath = Path.Combine(
                Folders.PhotoboxBaseDir,
                Folders.Deleted,
                imageName);

            Folders.CheckIfDirectoriesExistElseCreate();

            image.Save(newImagePath, new JpegEncoder());

            logger.LogInformation("Stored Deleted image under path{imagePath}", imageName);
        }
    }

    public async Task PrintAndSaveAsync(Image<Rgb24> image)
    {
        Folders.CheckIfDirectoriesExistElseCreate();

        Save(image);

        await printer.PrintAsync(image);
    }

    public void Save(Image<Rgb24> image)
    {
        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        Folders.CheckIfDirectoriesExistElseCreate();

        image.Save(newImagePath, new JpegEncoder());

        logger.LogInformation("Stored Saved image under path{imagePath}", imageName);

    }
}
