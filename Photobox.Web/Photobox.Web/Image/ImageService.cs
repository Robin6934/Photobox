using Microsoft.EntityFrameworkCore;
using Photobox.Web.DbContext;
using Photobox.Web.Models;
using Photobox.Web.StorageProvider;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Web.Image;

public class ImageService(MariaDbContext dbContext, IStorageProvider storageProvider, ILogger<ImageService> logger)
{
    private readonly MariaDbContext _dbContext = dbContext;

    private readonly IStorageProvider _storageProvider = storageProvider;

    private readonly ILogger<ImageService> _logger = logger;

    public async Task StoreImageAsync(Image<Rgb24> image, string imageName)
    {
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imageName);

        if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);
        }

        ImageModel imageModel = new()
        {
            UniqeImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}",
            ImageName = imageName,
            DownscaledImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}",
            TakenAt = DateTime.Now
        };

        int newWidth = image.Width < 1000 ? image.Width : 1000;

        using var downScaledImage = image.Clone(i => i.Resize(newWidth, 0));

        var saveLocallyTask = downScaledImage.SaveAsJpegAsync(imagePath);

        var storeDownscaledTask = _storageProvider.StoreImageAsync(downScaledImage, imageModel.DownscaledImageName);

        var fullSizeTask = _storageProvider.StoreImageAsync(image, imageModel.UniqeImageName);

        var dbTask = _dbContext.ImageModels.AddAsync(imageModel);

        await Task.WhenAll(saveLocallyTask, storeDownscaledTask, fullSizeTask, dbTask.AsTask());

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Image<Rgb24>> GetImageAsync(string imageName)
    {
        var imageModel = await _dbContext.ImageModels.Where(model => model.ImageName == imageName).FirstAsync();

        var image = await _storageProvider.GetImageAsync(imageModel.UniqeImageName);

        return image;
    }

    public async Task<Image<Rgb24>> GetPreviewImageAsync(string imageName)
    {
        var imageModel = await _dbContext.ImageModels.Where(model => model.ImageName == imageName).FirstAsync();

        var image = await _storageProvider.GetImageAsync(imageModel.DownscaledImageName);

        return image;
    }

    public Task<List<string>> ListImagesAsync()
    {
        return _dbContext.ImageModels.Select(image => image.ImageName).ToListAsync();
    }

    public async Task DeleteImagesAsync()
    {
        var images = await _dbContext.ImageModels.ToArrayAsync();

        foreach (var image in images)
        {
            await DeleteImageAsync(image.ImageName);
        }
    }

    public async Task DeleteImageAsync(string imageName)
    {
        var imageModel = await _dbContext.ImageModels.Where(image => image.ImageName == imageName).FirstOrDefaultAsync();

        if (imageModel is null)
        {
            return;
        }

        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imageName);

        await _storageProvider.DeleteImageAsync(imageName);

        await _storageProvider.DeleteImageAsync(imageName);

        File.Delete(imagePath);

        _dbContext.ImageModels.Remove(imageModel);

        await _dbContext.SaveChangesAsync();
    }
}
