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
    private readonly MockFileSystem fileSystem = new MockFileSystem(
        new Dictionary<string, MockFileData>
        {
            { Folders.Photos, new MockDirectoryData() },
            { Folders.Deleted, new MockDirectoryData() },
            { Folders.Temp, new MockDirectoryData() },
        },
        Folders.PhotoboxBaseDir
    );

    [Fact]
    public async Task PrintImage()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

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

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            Substitute.For<IOptionsMonitor<PhotoboxConfig>>(),
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        Image<Rgb24> image = new(200, 200);

        await imageService.PrintAndSaveAsync(image);

        fileSystem.Directory.GetFiles(Folders.GetPath(Folders.Photos)).Should().HaveCount(1);
    }

    [Fact]
    public async Task SaveImage()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var config = Substitute.For<IOptionsMonitor<PhotoboxConfig>>();
        config.CurrentValue.Returns(new PhotoboxConfig { StoreDeletedImages = true });

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            config,
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        Image<Rgb24> image = new(200, 200);

        await imageService.SaveAsync(image);

        fileSystem.Directory.GetFiles(Folders.GetPath(Folders.Photos)).Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteImage_StoreDeletedImages()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var config = Substitute.For<IOptionsMonitor<PhotoboxConfig>>();
        config.CurrentValue.Returns(new PhotoboxConfig { StoreDeletedImages = true });

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            config,
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        Image<Rgb24> image = new(200, 200);

        await imageService.DeleteAsync(image);

        fileSystem.Directory.GetFiles(Folders.GetPath(Folders.Deleted)).Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteImage_DontStoreDeletedImages()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var config = Substitute.For<IOptionsMonitor<PhotoboxConfig>>();
        config.CurrentValue.Returns(new PhotoboxConfig { StoreDeletedImages = false });

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            config,
            printer,
            Substitute.For<IImageUploadService>(),
            fileSystem
        );

        Image<Rgb24> image = new(200, 200);

        await imageService.DeleteAsync(image);

        fileSystem.Directory.GetFiles(Folders.GetPath(Folders.Deleted)).Should().HaveCount(0);
    }

    [Fact]
    public async Task SaveImage_ShouldUploadImage()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var imageUploadService = Substitute.For<IImageUploadService>();

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            Substitute.For<IOptionsMonitor<PhotoboxConfig>>(),
            printer,
            imageUploadService,
            fileSystem
        );

        Image<Rgb24> image = new(200, 200);

        await imageService.SaveAsync(image);

        await imageUploadService
            .Received(1)
            .UploadImageAsync(Arg.Any<string>(), Arg.Any<Image<Rgb24>>());
    }

    [Fact]
    public async Task PrintImage_ShouldUploadImage()
    {
        var printer = Substitute.For<IPrinter>();

        var logger = Substitute.For<ILogger<ImageManager.ImageManager>>();

        var imageUploadService = Substitute.For<IImageUploadService>();

        IImageManager imageService = new ImageManager.ImageManager(
            logger,
            Substitute.For<IOptionsMonitor<PhotoboxConfig>>(),
            printer,
            imageUploadService,
            fileSystem
        );

        Image<Rgb24> image = new(200, 200);

        await imageService.PrintAndSaveAsync(image);

        await imageUploadService
            .Received(1)
            .UploadImageAsync(Arg.Any<string>(), Arg.Any<Image<Rgb24>>());
    }
}
