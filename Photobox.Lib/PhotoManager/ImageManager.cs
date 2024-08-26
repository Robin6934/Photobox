using Photobox.Lib.Printer;

namespace Photobox.Lib.PhotoManager;
public class ImageManager(IPrinter printer) : IImageManager
{
    private IPrinter printer = printer;

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

    public async Task PrintAndSaveAsync(string imagePath, string printerName)
    {
        Folders.CheckIfDirectoriesExistElseCreate();

        await printer.PrintAsync(imagePath, printerName);

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

        File.Move(imagePath, newImagePath);
    }
}
