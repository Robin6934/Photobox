using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Photobox.Web.DbContext;
using Photobox.Web.Models;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection.Metadata.Ecma335;

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

        var image = await SixLabors.ImageSharp.Image.LoadAsync<Rgb24>(formFile.OpenReadStream());

        if(image is null)
        {
            return BadRequest("File has wrong format.");
        }

        await imageService.StoreImageAsync(image, imageName);

        return Ok();
    }

    [HttpGet("{imageName}")]
    public async Task<FileResult> GetImage(string imageName)
    {
        var image = await imageService.GetImageAsStreamAsync(imageName);

        return File(image, "image/jpeg", imageName);
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
}
