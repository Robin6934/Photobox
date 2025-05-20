using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Photobox.Web.Database;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Web.Services;

public class ImageService(
    AppDbContext dbContext,
    IStorageService storageService,
    ILogger<ImageService> logger,
    IMemoryCache memoryCache
)
{
    private readonly ILogger<ImageService> _logger = logger;

    public async Task StoreImageAsync(
        Models.Image imageModel,
        Image<Rgb24> image,
        CancellationToken cancellationToken = default
    )
    {
        int newWidth = image.Width < 1000 ? image.Width : 1000;

        using var downScaledImage = image.Clone(i => i.Resize(newWidth, 0));

        var storeDownscaledTask = storageService.StoreImageAsync(
            downScaledImage,
            imageModel.DownscaledImageName,
            cancellationToken
        );

        var fullSizeTask = storageService.StoreImageAsync(
            image,
            imageModel.UniqueImageName,
            cancellationToken
        );

        var dbTask = dbContext.Images.AddAsync(imageModel, cancellationToken);

        await Task.WhenAll(storeDownscaledTask, fullSizeTask, dbTask.AsTask());

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Image<Rgb24>> GetImageAsync(string imageName)
    {
        var imageModel = await dbContext.Images.FirstAsync(model => model.ImageName == imageName);

        var image = await storageService.GetImageAsync(imageModel.UniqueImageName);

        return image;
    }

    public async Task<Image<Rgb24>> GetPreviewImageAsync(string imageName)
    {
        var imageModel = await dbContext.Images.FirstAsync(model => model.ImageName == imageName);

        var image = await storageService.GetImageAsync(imageModel.DownscaledImageName);

        return image;
    }

    public Task<List<string>> ListImagesAsync()
    {
        return dbContext.Images.Select(image => image.ImageName).ToListAsync();
    }

    public Task DeleteImagesAsync(
        IEnumerable<Photobox.Web.Models.Image> images,
        CancellationToken cancellationToken = default
    )
    {
        var imageNames = images.SelectMany(i => new[] { i.UniqueImageName, i.DownscaledImageName });

        return storageService.DeleteImagesAsync(imageNames, cancellationToken);
    }

    public async Task DeleteImageAsync(string imageName)
    {
        var imageModel = await dbContext
            .Images.Where(image => image.ImageName == imageName)
            .FirstOrDefaultAsync();

        if (imageModel is null)
        {
            return;
        }

        await storageService.DeleteImageAsync(imageModel.UniqueImageName);

        await storageService.DeleteImageAsync(imageModel.DownscaledImageName);

        dbContext.Images.Remove(imageModel);

        await dbContext.SaveChangesAsync();
    }

    public async Task<string> GetPreviewImagePreSignedUrl(
        string imageName,
        TimeSpan validFor = default
    )
    {
        if (validFor == default)
        {
            validFor = TimeSpan.FromMinutes(30);
        }

        var imageModel = dbContext.Images.First(model => model.ImageName == imageName);

        if (!memoryCache.TryGetValue(imageModel.DownscaledImageName, out string? preSignedUrl))
        {
            preSignedUrl = await storageService.GetPreSignedUrl(
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
