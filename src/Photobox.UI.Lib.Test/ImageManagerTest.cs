using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
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

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>()
            {
                { Folders.GetPath(Folders.Photos), new MockDirectoryData() },
                { Folders.GetPath(Folders.Deleted), new MockDirectoryData() },
                { Folders.GetPath(Folders.Temp), new MockDirectoryData() },
            }
        );

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            Substitute.For<IOptionsMonitor<PhotoboxConfig>>(),
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(Folders.PhotoboxBaseDir, Folders.Photos, imageName);

        Image<Rgb24> image = new(200, 200);

        await imageService.PrintAndSaveAsync(image);

        await printer.Received(1).PrintAsync(image);
    }

    [Fact]
    public async Task PrintImageAndCheckIfItIsAlsoSaved()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>()
            {
                { Folders.GetPath(Folders.Photos), new MockDirectoryData() },
                { Folders.GetPath(Folders.Deleted), new MockDirectoryData() },
                { Folders.GetPath(Folders.Temp), new MockDirectoryData() },
            }
        );

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            Substitute.For<IOptionsMonitor<PhotoboxConfig>>(),
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(Folders.PhotoboxBaseDir, Folders.Photos, imageName);

        Image<Rgb24> image = new(200, 200);

        await imageService.PrintAndSaveAsync(image);

        fileSystem.Directory
            .GetFiles(Folders.GetPath(Folders.Photos))
            .Should()
            .HaveCount(1);    
    }

    [Fact]
    public async Task SaveImage()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>()
            {
                { Folders.GetPath(Folders.Photos), new MockDirectoryData() },
                { Folders.GetPath(Folders.Deleted), new MockDirectoryData() },
                { Folders.GetPath(Folders.Temp), new MockDirectoryData() },
            }
        );

        var config = Substitute.For<IOptionsMonitor<PhotoboxConfig>>();
        config.CurrentValue.Returns(new PhotoboxConfig { StoreDeletedImages = true });

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            config,
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(Folders.PhotoboxBaseDir, Folders.Photos, imageName);

        Image<Rgb24> image = new(200, 200);

        await imageService.SaveAsync(image);

        fileSystem.Directory
            .GetFiles(Folders.GetPath(Folders.Photos))
            .Should()
            .HaveCount(1);
    }

    [Fact]
    public async Task DeleteImage_StoreDeletedImages()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>()
            {
                { Folders.GetPath(Folders.Photos), new MockDirectoryData() },
                { Folders.GetPath(Folders.Deleted), new MockDirectoryData() },
                { Folders.GetPath(Folders.Temp), new MockDirectoryData() },
            }
        );
        var config = Substitute.For<IOptionsMonitor<PhotoboxConfig>>();
        config.CurrentValue.Returns(new PhotoboxConfig { StoreDeletedImages = true });

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            config,
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(Folders.PhotoboxBaseDir, Folders.Deleted, imageName);

        Image<Rgb24> image = new(200, 200);

        await imageService.DeleteAsync(image);

        fileSystem.Directory
            .GetFiles(Folders.GetPath(Folders.Deleted))
            .Should()
            .HaveCount(1);
    }
    
    [Fact]
    public async Task DeleteImage_DontStoreDeletedImages()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>()
            {
                { Folders.GetPath(Folders.Photos), new MockDirectoryData() },
                { Folders.GetPath(Folders.Deleted), new MockDirectoryData() },
                { Folders.GetPath(Folders.Temp), new MockDirectoryData() },
            }
        );
        var config = Substitute.For<IOptionsMonitor<PhotoboxConfig>>();
        config.CurrentValue.Returns(new PhotoboxConfig { StoreDeletedImages = false });

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            config,
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        string imageName = Folders.NewImageName;

        string newImagePath = Path.Combine(Folders.PhotoboxBaseDir, Folders.Deleted, imageName);

        Image<Rgb24> image = new(200, 200);

        await imageService.DeleteAsync(image);

        fileSystem.Directory
            .GetFiles(Folders.GetPath(Folders.Deleted))
            .Should()
            .HaveCount(0);
    }
}
