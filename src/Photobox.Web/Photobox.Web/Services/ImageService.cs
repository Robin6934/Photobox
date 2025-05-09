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
        Image<Rgb24> image,
        string imageName,
        string photoboxHardwareId
    )
    {
        var photobox = dbContext
            .PhotoBoxes.Include(photobox => photobox.Event)
            .Single(photoBox => photoBox.HardwareId == photoboxHardwareId);

        Models.Image imageModel = new()
        {
            Id = Guid.CreateVersion7(),
            UniqueImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}",
            ImageName = imageName,
            DownscaledImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}",
            //TODO: 1.12.2024 cant use dateTime.now with postgress need to fix later
            TakenAt = DateTime.UtcNow,
            Event = photobox.Event,
            PhotoboxHardwareId = photoboxHardwareId,
        };

        int newWidth = image.Width < 1000 ? image.Width : 1000;

        using var downScaledImage = image.Clone(i => i.Resize(newWidth, 0));

        var storeDownscaledTask = storageService.StoreImageAsync(
            downScaledImage,
            imageModel.DownscaledImageName
        );

        var fullSizeTask = storageService.StoreImageAsync(image, imageModel.UniqueImageName);

        var dbTask = dbContext.Images.AddAsync(imageModel);

        await Task.WhenAll(storeDownscaledTask, fullSizeTask, dbTask.AsTask());

        await dbContext.SaveChangesAsync();
    }

    public async Task<Image<Rgb24>> GetImageAsync(string imageName)
    {
        var imageModel = await dbContext
            .Images.Where(model => model.ImageName == imageName)
            .FirstAsync();

        var image = await storageService.GetImageAsync(imageModel.UniqueImageName);

        return image;
    }

    public async Task<Image<Rgb24>> GetPreviewImageAsync(string imageName)
    {
        var imageModel = await dbContext
            .Images.Where(model => model.ImageName == imageName)
            .FirstAsync();

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

        var imageModel = dbContext.Images.Where(model => model.ImageName == imageName).First();

        if (!memoryCache.TryGetValue(imageModel.DownscaledImageName, out string preSignedUrl))
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
