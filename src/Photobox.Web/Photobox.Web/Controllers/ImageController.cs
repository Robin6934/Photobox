using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Photobox.Lib.Extensions;
using Photobox.Web.Responses;
using Photobox.Web.Services;
using SixLabors.ImageSharp.PixelFormats;
using System.Net;

namespace Photobox.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ImageController(ImageService imageService) : Controller
{
    /// <summary>
    /// Uploads a picture to the server.
    /// </summary>
    /// <param name="photoBoxId">The unique identifier of the photobox that has taken the picture.</param>
    /// <param name="formFile">The picture file to upload.</param>
    /// <response code="200">Image has been uploaded successfully</response>
    [HttpPost]
    [ProducesResponseType<ImageUploadResponse>((int)HttpStatusCode.OK)]
    [Authorize]
    public async Task<IActionResult> UploadImage(
        [FromHeader(Name = "X-PhotoBox-Id")] string photoBoxId,
        IFormFile formFile
    )
    {
        if (formFile == null || formFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using var image = await SixLabors.ImageSharp.Image.LoadAsync<Rgb24>(
            formFile.OpenReadStream()
        );

        if (image is null)
        {
            return BadRequest("File has wrong format.");
        }

        await imageService.StoreImageAsync(image, formFile.FileName);

        return Ok(new ImageUploadResponse { FileName = formFile.FileName });
    }

    [HttpGet("{imageName}")]
    [ProducesResponseType<FileStreamResult>((int)HttpStatusCode.OK)]
    public async Task<FileStreamResult> GetImage(string imageName)
    {
        var image = await imageService.GetImageAsync(imageName);

        return File(await image.ToJpegStreamAsync(), "image/jpeg", imageName);
    }

    [HttpGet("{imageName}")]
    [ProducesResponseType<FileStreamResult>((int)HttpStatusCode.OK)]
    public async Task<FileStreamResult> GetPreviewImage(string imageName)
    {
        var image = await imageService.GetPreviewImageAsync(imageName);

        return File(await image.ToJpegStreamAsync(), "image/jpeg", imageName);
    }

    [HttpGet("{imageName}")]
    public string GetPreviewImagePreSignedUrl(string imageName)
    {
        return imageService.GetPreviewImagePreSignedUrl(imageName, TimeSpan.FromMinutes(30));
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
