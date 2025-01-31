using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Photobox.UI.Lib.ConfigModels;
using Photobox.UI.Lib.ImageManager;
using Photobox.UI.Lib.ImageUploadService;
using Photobox.UI.Lib.Printer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.Test;

public class ImageManagerTest
{
    [Fact]
    public async Task PrintImage()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        IImageManager imageService = new ImageManager.ImageManager(logger, Substitute.For<IOptionsMonitor<PhotoboxConfig>>(), printer, Substitute.For<IImageUploadService>());

        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            Image<Rgb24> image = new(200, 200);

            await imageService.PrintAndSaveAsync(image);

            await printer.Received(1).PrintAsync(image);
        }
        finally
        {
            DeleteFileIfExists(newImagePath);
            DeleteFileIfExists(imageName);
        }
    }

    [Fact(Skip = "does not work")]
    public async Task PrintImageAndCheckIfItIsAlsoSaved()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        IImageManager imageService = new ImageManager.ImageManager(logger, Substitute.For<IOptionsMonitor<PhotoboxConfig>>(), printer, Substitute.For<IImageUploadService>());

        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            Image<Rgb24> image = new(200, 200);

            await imageService.PrintAndSaveAsync(image);

            CheckIfFileExists(newImagePath);
        }
        finally
        {
            DeleteFileIfExists(newImagePath);
            DeleteFileIfExists(imageName);
        }
    }

    [Fact]
    public async Task SaveImage()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        IImageManager imageService = new ImageManager.ImageManager(logger, Substitute.For<IOptionsMonitor<PhotoboxConfig>>(), printer, Substitute.For<IImageUploadService>());

        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Photos,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            Image<Rgb24> image = new(200, 200);

            await imageService.SaveAsync(image);

            CheckIfFileExists(newImagePath);
        }
        finally
        {
            DeleteFileIfExists(newImagePath);
            DeleteFileIfExists(imageName);
        }
    }

    [Fact(Skip = "does not work")]
    public void DeleteImage()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        IImageManager imageService = new ImageManager.ImageManager(logger, Substitute.For<IOptionsMonitor<PhotoboxConfig>>(), printer, Substitute.For<IImageUploadService>());

        string imageName = Folders.NewImageName;

        Folders.NewImageName.Returns(imageName);

        string newImagePath = Path.Combine(
            Folders.PhotoboxBaseDir,
            Folders.Deleted,
            imageName);

        try
        {
            DeleteFileIfExists(imageName);

            Image<Rgb24> image = new(200, 200);

            imageService.DeleteAsync(image);

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