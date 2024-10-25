using Microsoft.Extensions.Logging;
using Photobox.Lib.Printer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Lib.PhotoManager;
public class ImageManager(ILogger<ImageManager> logger, IPrinter printer) : IImageManager
{
    private readonly ILogger<ImageManager> logger = logger;

    private readonly IPrinter printer = printer;

    public void Delete(Image<Rgb24> image)
    {
        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Deleted,
            imageName);

        Folders.CheckIfDirectoriesExistElseCreate();

        image.Save(newImagePath, new JpegEncoder());
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
    }
}
