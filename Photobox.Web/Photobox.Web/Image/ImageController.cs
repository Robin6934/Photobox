using Microsoft.AspNetCore.Mvc;
using Photobox.Lib.Extensions;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Net;

namespace Photobox.Web.Image;
[ApiController]
[Route("api/[controller]/[action]")]
public class ImageController(ImageService imageService) : Controller
{
    private readonly ImageService imageService = imageService;

    /// <summary>
    /// Uploads a picture to the server.
    /// </summary>
    /// <param name="formFile">The picture file to upload.</param>
    /// <param name="imageName">The Name of the Picture to upload.</param>
    /// <returns>Returns a 200 OK response if successful or a 400 Bad Request if no file is provided.</returns>
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile formFile, string imageName)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using var image = await SixLabors.ImageSharp.Image.LoadAsync<Rgb24>(formFile.OpenReadStream());
        ReadMetadata(image);
        if (image is null)
        {
            return BadRequest("File has wrong format.");
        }

        await imageService.StoreImageAsync(image, imageName);

        return Ok();
    }

    private static void ReadMetadata(SixLabors.ImageSharp.Image<Rgb24> image)
    {
        if (image.Metadata.ExifProfile?.Values?.Any() ?? false)
        {
            foreach (var prop in image.Metadata.ExifProfile.Values)
                Debug.WriteLine($"{prop.Tag}: {prop.DataType}, {prop.GetValue()}");
        }
    }

    [HttpGet("{imageName}")]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<FileStreamResult> GetImage(string imageName)
    {
        var image = await imageService.GetImageAsync(imageName);

        return File(await image.ToJpegStreamAsync(), "image/jpeg", imageName);
    }

    [HttpGet("{imageName}")]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<FileStreamResult> GetPreviewImage(string imageName)
    {
        var image = await imageService.GetPreviewImageAsync(imageName);

        return File(await image.ToJpegStreamAsync(), "image/jpeg", imageName);
    }

    [HttpGet]
    public Task<List<string>> ListImages()
    {
        return imageService.ListImagesAsync();
    }

    [HttpDelete]
    public Task DeleteImages()
    {
        return imageService.DeleteImagesAsync();
    }

    [HttpDelete]
    public Task DeleteImage(string imageName)
    {
        return imageService.DeleteImageAsync(imageName);
    }
}
