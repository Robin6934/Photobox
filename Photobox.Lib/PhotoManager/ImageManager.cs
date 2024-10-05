using Microsoft.Extensions.Logging;
using Photobox.Lib.Printer;

namespace Photobox.Lib.PhotoManager;
public class ImageManager(ILogger<ImageManager> logger, IPrinter printer) : IImageManager
{
    private readonly ILogger<ImageManager> logger = logger;

    private readonly IPrinter printer = printer;

    public void Delete(string imagePath)
    {
        string imageName = Path.GetFileName(imagePath);

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Deleted,
            imageName);

        Folders.CheckIfDirectoriesExistElseCreate();

        File.Move(imagePath, newImagePath);
    }

    public async Task PrintAndSaveAsync(string imagePath)
    {
        Folders.CheckIfDirectoriesExistElseCreate();

        await printer.PrintAsync(imagePath);

        Save(imagePath);
    }

    public void Save(string imagePath)
    {
        string imageName = Path.GetFileName(imagePath);

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        Folders.CheckIfDirectoriesExistElseCreate();

        File.Move(imagePath, newImagePath, false);
    }
}
