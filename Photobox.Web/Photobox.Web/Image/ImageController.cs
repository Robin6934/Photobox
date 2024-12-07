using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Photobox.Lib.Extensions;
using SixLabors.ImageSharp.PixelFormats;
using System.Net;

namespace Photobox.Web.Image;

[ApiController]
[Route("api/[controller]/[action]")]
public class ImageController(ImageService imageService) : Controller
{
    /// <summary>
    /// Uploads a picture to the server.
    /// </summary>
    /// <param name="formFile">The picture file to upload.</param>
    /// <response code="200">Image has been uploaded successfully</response>
    [HttpPost]
    [ProducesResponseType<ImageUploadResult>((int)HttpStatusCode.OK)]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> UploadImage(IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using var image = await SixLabors.ImageSharp.Image.LoadAsync<Rgb24>(formFile.OpenReadStream());

        if (image is null)
        {
            return BadRequest("File has wrong format.");
        }

        await imageService.StoreImageAsync(image, formFile.FileName);

        return Ok(new ImageUploadResult { FileName = formFile.FileName });
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
