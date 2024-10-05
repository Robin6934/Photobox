using Microsoft.Extensions.Logging;
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

        var logger = Substitute.For<ILogger<ImageManager>>();

        IImageManager imageService = new ImageManager(logger, printer);

        string imageName = "testImage.jpg";

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            File.WriteAllText(imageName, "testFileContent");

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

        var logger = Substitute.For<ILogger<ImageManager>>();

        IImageManager imageService = new ImageManager(logger, printer);

        string imageName = "testImage.jpg";

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            File.WriteAllText(imageName, "testFileContent");

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

        var logger = Substitute.For<ILogger<ImageManager>>();

        IImageManager imageService = new ImageManager(logger, printer);

        string imageName = "testImage.jpg";

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            File.WriteAllText(imageName, "testFileContent");

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

        var logger = Substitute.For<ILogger<ImageManager>>();

        IImageManager imageService = new ImageManager(logger, printer);

        string imageName = "testImage.jpg";

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Deleted,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            File.WriteAllText(imageName, "testFileContent");

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