using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Photobox.Web.DbContext;
using Photobox.Web.StorageProvider;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Web.Image;

public class ImageService(
    AppDbContext dbContext,
    IStorageProvider storageProvider,
    ILogger<ImageService> logger,
    IMemoryCache memoryCache
)
{
    private readonly ILogger<ImageService> _logger = logger;

    public async Task StoreImageAsync(Image<Rgb24> image, string imageName)
    {
        ImageModel imageModel = new()
        {
            UniqueImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}",
            ImageName = imageName,
            DownscaledImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}",
            //TODO: 1.12.2024 cant use dateTime.now with postgress need to fix later
            TakenAt = DateTime.UtcNow,
        };

        int newWidth = image.Width < 1000 ? image.Width : 1000;

        using var downScaledImage = image.Clone(i => i.Resize(newWidth, 0));

        var storeDownscaledTask = storageProvider.StoreImageAsync(
            downScaledImage,
            imageModel.DownscaledImageName
        );

        var fullSizeTask = storageProvider.StoreImageAsync(image, imageModel.UniqueImageName);

        var dbTask = dbContext.ImageModels.AddAsync(imageModel);

        await Task.WhenAll(storeDownscaledTask, fullSizeTask, dbTask.AsTask());

        await dbContext.SaveChangesAsync();
    }

    public async Task<Image<Rgb24>> GetImageAsync(string imageName)
    {
        var imageModel = await dbContext
            .ImageModels.Where(model => model.ImageName == imageName)
            .FirstAsync();

        var image = await storageProvider.GetImageAsync(imageModel.UniqueImageName);

        return image;
    }

    public async Task<Image<Rgb24>> GetPreviewImageAsync(string imageName)
    {
        var imageModel = await dbContext
            .ImageModels.Where(model => model.ImageName == imageName)
            .FirstAsync();

        var image = await storageProvider.GetImageAsync(imageModel.DownscaledImageName);

        return image;
    }

    public Task<List<string>> ListImagesAsync()
    {
        return dbContext.ImageModels.Select(image => image.ImageName).ToListAsync();
    }

    public async Task DeleteImagesAsync()
    {
        var images = await dbContext.ImageModels.ToArrayAsync();

        foreach (var image in images)
        {
            await DeleteImageAsync(image.ImageName);
        }
    }

    public async Task DeleteImageAsync(string imageName)
    {
        var imageModel = await dbContext
            .ImageModels.Where(image => image.ImageName == imageName)
            .FirstOrDefaultAsync();

        if (imageModel is null)
        {
            return;
        }

        await storageProvider.DeleteImageAsync(imageName);

        await storageProvider.DeleteImageAsync(imageName);

        dbContext.ImageModels.Remove(imageModel);

        await dbContext.SaveChangesAsync();
    }

    public string GetPreviewImagePreSignedUrl(string imageName, TimeSpan validFor = default)
    {
        if (validFor == default)
        {
            validFor = TimeSpan.FromMinutes(30);
        }

        var imageModel = dbContext.ImageModels.Where(model => model.ImageName == imageName).First();

        if (!memoryCache.TryGetValue(imageModel.DownscaledImageName, out string preSignedUrl))
        {
            preSignedUrl = storageProvider.GetPreSignedUrl(
                imageModel.DownscaledImageName,
                validFor
            );

            //Set the timeout a little lower, so it wont be invalidated, the second the client receives it
            memoryCache.Set(imageModel.DownscaledImageName, preSignedUrl, validFor * 0.95);
        }

        ArgumentException.ThrowIfNullOrEmpty(preSignedUrl);

        return preSignedUrl;
    }
}
