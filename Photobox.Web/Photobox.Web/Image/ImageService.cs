using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.DbContext;
using Photobox.Web.Models;
using Photobox.Web.StorageProvider;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Photobox.Web.Image;

public class ImageService(MariaDbContext dbContext, IStorageProvider storageProvider, ILogger<ImageService> logger)
{
    private readonly MariaDbContext dbContext = dbContext;

    private readonly IStorageProvider storageProvider = storageProvider;

    private readonly ILogger<ImageService> logger = logger;

    public async Task StoreImageAsync(Image<Rgb24> image, string imageName)
    {
        ImageModel imageModel = new()
        {
            UniqeImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}",
            ImageName = imageName,
            DownscaledImageName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}"
        };

        await storageProvider.StoreImageAsync(image.Clone(i => i.Resize(image.Width / 4, 0)), imageModel.DownscaledImageName);

        await storageProvider.StoreImageAsync(image, imageModel.UniqeImageName);

        await dbContext.ImageModels.AddAsync(imageModel);

        await dbContext.SaveChangesAsync();
    }

    public async Task<Stream> GetImageAsStreamAsync(string imageName)
    {
        var imageModel = await dbContext.ImageModels.Where(model => model.ImageName == imageName).FirstAsync();

        var image = await storageProvider.GetImageAsync(imageModel.UniqeImageName);

        var imageStream = new MemoryStream();

        await image.SaveAsJpegAsync(imageStream);

        return imageStream;
    }

    public Task<List<string>> ListImagesAsync()
    {
        return dbContext.ImageModels.Select(image => image.ImageName).ToListAsync();
    }

    public async Task DeleteImagesAsync()
    {
        var images = dbContext.ImageModels.ToList();

        foreach(var image in images)
        {
            await storageProvider.DeleteImageAsync(image.UniqeImageName);

            dbContext.ImageModels.Remove(image);

            await dbContext.SaveChangesAsync();
        }
    }
}
