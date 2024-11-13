using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore;
using Photobox.Lib.Extensions;
using Photobox.Web.DbContext;
using Photobox.Web.Models;
using Photobox.Web.StorageProvider;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;

namespace Photobox.Web.Image;

public class ImageService(MariaDbContext dbContext, IStorageProvider storageProvider, ILogger<ImageService> logger)
{
    private readonly MariaDbContext dbContext = dbContext;

    private readonly IStorageProvider storageProvider = storageProvider;

    private readonly ILogger<ImageService> logger = logger;

    public async Task StoreImageAsync(Image<Rgb24> image, string imageName)
    {
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imageName);

        if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
        }

        ImageModel imageModel = new()
        {
            UniqeImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}",
            ImageName = imageName,
            DownscaledImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}"
        };

        using var downScaledImage = image.Clone(i => i.Resize(image.Width / 4, 0));

        var saveLocallyTask = downScaledImage.SaveAsJpegAsync(imagePath);

        var storeDownscaledTask = storageProvider.StoreImageAsync(downScaledImage, imageModel.DownscaledImageName);

        var storeFullsizeTask = storageProvider.StoreImageAsync(image, imageModel.UniqeImageName);

        var dbTask = dbContext.ImageModels.AddAsync(imageModel);

        await Task.WhenAll(saveLocallyTask, storeDownscaledTask, storeFullsizeTask, dbTask.AsTask());

        await dbContext.SaveChangesAsync();
    }

    public async Task<Image<Rgb24>> GetImageAsync(string imageName)
    {
        var imageModel = await dbContext.ImageModels.Where(model => model.ImageName == imageName).FirstAsync();

        var image = await storageProvider.GetImageAsync(imageModel.UniqeImageName);

        return image;
    }

    public async Task<Image<Rgb24>> GetPreviewImageAsync(string imageName)
    {
        var imageModel = await dbContext.ImageModels.Where(model => model.ImageName == imageName).FirstAsync();

        var image = await storageProvider.GetImageAsync(imageModel.DownscaledImageName);

        return image;
    }

    public Task<List<string>> ListImagesAsync()
    {
        return dbContext.ImageModels.Select(image => image.ImageName).ToListAsync();
    }

    public async Task DeleteImagesAsync()
    {
        var images = dbContext.ImageModels.ToList();

        foreach (var image in images)
        {
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", image.ImageName);

            await storageProvider.DeleteImageAsync(image.UniqeImageName);

            await storageProvider.DeleteImageAsync(image.DownscaledImageName);

            File.Delete(imagePath);

            dbContext.ImageModels.Remove(image);

            await dbContext.SaveChangesAsync();
        }
    }
}
