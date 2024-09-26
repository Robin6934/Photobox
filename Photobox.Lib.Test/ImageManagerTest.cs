using NSubstitute;
using Photobox.Lib.PhotoManager;
using Photobox.Lib.Printer;

namespace Photobox.Lib.Test;

public class ImageManagerTest
{
    [Fact]
    public async Task PrintImage()
    {
        var printer = Substitute.For<IPrinter>();

        IImageManager imageService = new ImageManager(printer);

        string imageName = "testimage.jpg";

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            File.WriteAllText(imageName, "testfilecontent");

            await imageService.PrintAndSaveAsync(imageName);

            await printer.Received(1).PrintAsync(Arg.Any<string>());
        }
        finally
        {
            DeleteFileIfExists(newImagePath);
            DeleteFileIfExists(imageName);
        }
    }

    [Fact]
    public async Task PrintImageAndCheckIfItIsAlsoSaved()
    {
        var printer = Substitute.For<IPrinter>();

        IImageManager imageService = new ImageManager(printer);

        string imageName = "testimage.jpg";

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            File.WriteAllText(imageName, "testfilecontent");

            await imageService.PrintAndSaveAsync(imageName);

            CheckIfFileExists(newImagePath);
        }
        finally
        {
            DeleteFileIfExists(newImagePath);
            DeleteFileIfExists(imageName);
        }
    }

    [Fact]
    public void SaveImage()
    {
        var printer = Substitute.For<IPrinter>();

        IImageManager imageService = new ImageManager(printer);

        string imageName = "testimage.jpg";

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            File.WriteAllText(imageName, "testfilecontent");

            imageService.Save(imageName);

            CheckIfFileExists(newImagePath);
        }
        finally
        {
            DeleteFileIfExists(newImagePath);
            DeleteFileIfExists(imageName);
        }
    }

    [Fact]
    public void DeleteImage()
    {
        var printer = Substitute.For<IPrinter>();

        IImageManager imageService = new ImageManager(printer);

        string imageName = "testimage.jpg";

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Deleted,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            File.WriteAllText(imageName, "testfilecontent");

            imageService.Delete(imageName);

            CheckIfFileExists(newImagePath);
        }
        finally
        {
            DeleteFileIfExists(newImagePath);
            DeleteFileIfExists(imageName);
        }
    }

    private static void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private static void CheckIfFileExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"the file {filePath}, is not found!");
        }
    }
}